# TalaanNiAna: Improvement and Deployment Plan

**Repo:** `C:\Users\KenChami\source\repos\TalaanNiAna` (GitHub: `KenChamii/TalaanNiAna`)
**Stack today:** Angular 18 (standalone + signals) / ASP.NET Core .NET 10 (Clean Architecture, EF Core, Identity, JWT) / SQL Server LocalDB
**Goal:** a deployed, publicly demoable app that holds up to a senior developer reading the source.

---

## 0. Honest assessment

The architecture is genuinely good. Four-project Clean Architecture split, repository + unit of work with a real DB transaction around checkout, soft deletes that preserve transaction history, lazy-loaded standalone routes, typed DTOs, an exception middleware. The comment in `TransactionRepository.GetTopProductsThisMonthAsync` explaining why the query is flattened before `GroupBy` is the kind of thing reviewers notice and like.

What holds it back is not architecture. It is three things:

1. **It cannot be deployed as-is.** LocalDB connection string, hardcoded `localhost:4200` CORS, a placeholder API URL in `environment.prod.ts`, and a JWT signing key committed to `appsettings.json`.
2. **It is not actually multi-tenant.** Registration collects a `StoreName`, but no business entity carries a user or store id. The moment two people register, they share one inventory, one customer list, and one utang ledger.
3. **Several correctness and trust bugs** that a reviewer will find in ten minutes, listed in Phase 1.

Confirmed by reading the files: still on SQL Server (`Microsoft.EntityFrameworkCore.SqlServer` 10.0.9, `UseSqlServer` in both `Program.cs` and `AppDbContextFactory.cs`, both migrations full of `SqlServer:Identity` annotations). `main` is at `9b1b224`; there is no landing page or `/app/*` route structure on `main` or on any of the 16 remote branches.

---

## Phase 1: Correctness and security (do first, roughly 1 week)

These are not polish. Each one is a defect a reviewer can demonstrate.

### 1.1 Checkout trusts client-supplied prices (critical)

`CheckoutService.ProcessSaleAsync` computes `TotalAmount` from `dto.Items.Sum(i => i.Quantity * i.UnitPrice)` and stores `item.UnitPrice` straight from the request. A crafted POST with `unitPrice: 0.01` sells anything for one centavo.

**Fix:** read the price from the `Product` row server-side. Keep the client price only to compare, and reject with a "price changed, please re-scan" error if it differs. The `TransactionItem.UnitPrice` snapshot is correct design, it just needs to be the server's number.

### 1.2 Oversell under concurrency (critical)

`product.StockQuantity -= item.Quantity` is a read-modify-write with no concurrency control. Two simultaneous sales of the last item both succeed and stock goes negative.

**Fix:** add an optimistic concurrency token on `Product` (`xmin` on Postgres) and surface `DbUpdateConcurrencyException` as a 409. Alternatively a conditional update (`UPDATE ... SET StockQuantity = StockQuantity - @qty WHERE Id = @id AND StockQuantity >= @qty`), treating zero rows affected as insufficient stock.

### 1.3 Useful error messages are swallowed

`ExceptionHandlingMiddleware` turns every exception into a 500 with the generic "May naganap na error sa server." So "Not enough stock for Coke 1.5L. Available: 2, requested: 5." never reaches the cashier, even though checkout throws exactly that message.

**Fix:** map exception types to status codes. Add `DomainException` / `NotFoundException` / `ConflictException` in Application, return 400/404/409 with the message, reserve the generic 500 for genuinely unexpected failures, and use RFC 7807 ProblemDetails so the Angular interceptor has one shape to read.

### 1.4 Secrets in source control

`appsettings.json` contains `"Key": "your-super-secret-key-at-least-32-chars-long!"`, and `builder.Configuration["Jwt:Key"]!` null-forgives its way to a weak key or a crash.

**Fix:** strip the value, keep the structure. `dotnet user-secrets` locally, environment variables in production. Validate at startup via the options pattern with `ValidateOnStart` that the key exists and is at least 32 bytes.

### 1.5 Auth hardening

`UserManager.CheckPasswordAsync` is called directly, which bypasses Identity lockout entirely. Tokens last 7 days with no refresh and no revocation. `/register` and `/login` have no rate limiting, so brute force and signup spam are both free.

**Fix:** `SignInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true)` with lockout configured; the built-in .NET rate limiter on auth endpoints; access token down to 15 to 60 minutes with a persisted, revocable refresh token in an httpOnly Secure SameSite cookie. Move the Tagalog strings out of the controller into localization resources.

### 1.6 Token in localStorage

`AuthService` keeps the JWT in `localStorage`, readable by any injected script. With a refresh cookie from 1.5 the access token can live in a signal in memory. If you keep localStorage for now, document the tradeoff and the reason in the README, which reads much better than silence.

### 1.7 No 401 handling on the frontend

`error.interceptor` calls `alert()` while a fully written `ToastService` and `ToastComponent` sit unused, and nothing handles 401, so an expired token leaves the user stuck on failing requests.

**Fix:** use the toast service; on 401 clear auth state and redirect to `/login`; on 403 show a permission message.

### 1.8 Input validation is absent

DTOs carry no constraints. Negative prices, negative quantities, empty names, and 10,000-character names all pass.

**Fix:** FluentValidation in the Application layer wired as a filter so every endpoint validates uniformly, with matching Angular validators.

### 1.9 Timezone correctness

`DateTime.UtcNow.Date` defines "today" and `new DateTime(Year, Month, 1)` defines "this month". A Manila store (UTC+8) sees daily sales reset at 8:00 in the morning, with its last 8 hours counted in the next day.

**Fix:** store UTC (`timestamptz`), aggregate against `Asia/Manila` boundaries, and make the timezone a `StoreSettings` field so the design is deliberate rather than accidentally right.

### 1.10 Blocking call in a service constructor

`DashboardService`'s constructor does `settingsRepo.GetAsync().GetAwaiter().GetResult()`. The inline comment calls it safe, but it is sync-over-async on a request thread: thread-pool starvation under load and a known deadlock pattern.

**Fix:** await the settings read inside `GetSummaryAsync`, or cache properly with `IMemoryCache` behind an async accessor.

### 1.11 In-memory aggregation

Three queries pull whole tables into memory to compute one number: `GetTotalOutstandingCreditAsync` fetches every customer with `pageSize: int.MaxValue` then sums; `GetTodayTotalsAsync` materializes the day's rows then sums by payment type; `GetMonthlyAverageTransactionAsync` materializes the month then averages.

**Fix:** push these into SQL with `SumAsync`, `CountAsync`, `AverageAsync` and conditional sums. Reviewers specifically grep for `int.MaxValue` page sizes.

### 1.12 Credit balance can drift

`Customer.TotalCredit` is a denormalized running total mutated by `UpdateCreditAsync`. Any failed partial write, manual DB edit, or future void feature desynchronizes it, and nothing detects that.

**Fix:** derive the balance from the ledger (credit transactions plus `CreditPayment` rows), or keep the cached column and add a reconciliation query plus a test asserting cached equals derived.

### 1.13 Double-submit on checkout

A slow connection plus an impatient second tap posts the sale twice: two transactions, stock deducted twice.

**Fix:** accept a client-generated idempotency key, store it unique-indexed with the transaction, return the original result on replay. Disable the button in-flight too, but the server guarantee is the one that counts. This also becomes the foundation for offline sync in 4.1.

### 1.14 Small cleanups

- `Customer.cs` has a stray `using System.Transactions;` shadowing the domain's own `Transaction`. Remove it.
- `Products.cs` declares `class Product`. Rename to `Product.cs`.
- `Product.LowStockThreshold` (per product) and `StoreSettings.LowStockAlertLimit` (global) both exist; the dashboard only uses the global one. Make per-product override global, consistently.
- `GetQuickItemsAsync` has no `Take`, so it returns the whole active catalog ordered by stock. Cap it.

---

## Phase 2: Multi-tenancy (roughly 1 week, blocks public deployment)

**Current reality:** `ApplicationUser` has `DisplayName` and `StoreName`. `Product`, `Customer`, `Transaction`, `CreditPayment`, and `StoreSettings` have no owner, and `StoreSettings` is seeded with a fixed `Id = 1`, so there is exactly one store by construction.

1. Add `Store` (Id, Name, TimeZone, CreatedAt) and `ApplicationUser.StoreId`. On register, create the store and link the user.
2. Add indexed `StoreId` to every business entity, and make it part of each unique constraint.
3. Add a `storeId` claim to the JWT, resolved once per request into a scoped `ICurrentStore`.
4. Enforce in one place with an EF Core **global query filter** (`HasQueryFilter(p => p.StoreId == _currentStore.Id)`) and auto-set `StoreId` on insert by overriding `SaveChangesAsync`. Enforcement that cannot be forgotten is worth describing in the README.
5. Write the test that proves it: Store A's token must not read, update, or delete Store B's product by guessing an id. Right now it can.
6. Replace the `StoreSettings` `HasData` seed with per-store settings created at registration.

**Migration note:** you have only two migrations and the deployment target is a fresh database, so squash to one clean Postgres `InitialCreate` with tenancy already baked in. Do not port the SQL Server migrations.

---

## Phase 3: Deploy it (roughly 3 to 4 days)

Mirror the FrierenHR setup, which is already proven and free.

### 3.1 Move to PostgreSQL (Neon, Singapore)

- Swap `Microsoft.EntityFrameworkCore.SqlServer` for `Npgsql.EntityFrameworkCore.PostgreSQL`; `UseSqlServer` to `UseNpgsql` in `Program.cs` and `AppDbContextFactory.cs`.
- `decimal(18,2)` carries over cleanly as `numeric(18,2)`.
- `timestamptz` for timestamps, pairing with the 1.9 fix.
- Concurrency token becomes `xmin` (`.IsRowVersion().HasColumnName("xmin").HasColumnType("xid")`).
- Postgres `LIKE` is case-sensitive, so `p.Name.Contains(search)` needs `EF.Functions.ILike` or product search silently stops matching.
- Regenerate migrations against Postgres from scratch.

### 3.2 Configuration

- `appsettings.json`: structure only, no secrets.
- Render env vars: `ConnectionStrings__DefaultConnection`, `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`, `Cors__AllowedOrigins`.
- Replace the hardcoded CORS policy with the configured origin list.
- Add `UseHttpsRedirection`, HSTS, `app.MapHealthChecks("/health")` with a DB check.
- Serilog with structured JSON output and a request correlation id.

### 3.3 API on Render

- Multi-stage `Dockerfile` (SDK build, `mcr.microsoft.com/dotnet/aspnet:10.0` runtime, non-root user).
- Apply migrations as a deploy step or one-off job, not automatically at app startup. Auto-migrate on boot is a habit reviewers flag.
- Point the health check at `/health` so Render restarts a wedged instance.
- Free-tier reality: the instance sleeps after about 15 minutes idle, so the first request takes 30 to 50 seconds. Handle it gracefully (3.6) rather than letting it look broken.

### 3.4 Frontend on Cloudflare Workers

- Build-time config for the API URL instead of the committed `your-api-domain.com`.
- SPA fallback so a deep link like `/app/customers/5` survives a refresh.
- Hashed assets immutable, `index.html` no-cache.
- Suggested host: `talaanniana.irreversiblechain.workers.dev`, consistent with FrierenHR.

### 3.5 CI/CD with GitHub Actions

- On PR: `dotnet build` plus `dotnet test`, `npm ci` plus production build plus `ng test --no-watch`, and a pending-migration check.
- On tag: deploy API to Render, frontend to Cloudflare. Fits your version-tag-per-phase convention.
- Dependabot for npm and NuGet.

### 3.6 Demo mode (do not skip)

Nobody reviewing a portfolio will create an account.

- A **"Try the demo"** button on login that signs into a pre-seeded store with a month of realistic sales, your 30-plus products across six categories, customers carrying utang, and some low-stock items so the dashboard looks alive.
- Nightly reset so it survives visitors poking at it.
- A first-load state that explains the cold start ("Waking up the server, about 30 seconds on the free tier") instead of a spinner that looks hung.

---

## Phase 4: Features

Grouped by what each one buys you. The flagship tier is where the project stops looking like a CRUD exercise.

### Tier A: flagship features

#### 4.1 Tingi and repacking (the most domain-authentic feature available)

This is the defining sari-sari behavior and the model currently cannot express it. Stores buy a sack of rice, a box of shampoo sachets, or a case of soft drinks, then sell per cup, per piece, per bottle. Right now `Product` has one `Price` and one `StockQuantity`, so a sack and a cup of rice have to be two unrelated products whose stock never reconciles.

- `Product` gets a base unit, a selling unit, and a conversion factor (1 sack = 25 kg = 50 takal).
- A **repack transaction** converts stock from one unit to another and records the conversion, so 1 sack leaving becomes 50 takal arriving, and the cost per selling unit is derived rather than guessed.
- Checkout sells in the selling unit; the ledger stays balanced in the base unit.
- Handle the awkward real case: partial packs, spillage during repacking, and rounding on the cost split.

Nobody building a generic POS tutorial has this. It demonstrates you modeled the actual business rather than a shopping cart.

#### 4.2 Offline-first PWA

Sari-sari stores run on cheap Android phones with patchy data. An offline-capable POS proves you understood the user.

- Angular service worker for app shell and assets.
- Queue checkouts in IndexedDB when offline, sync on reconnect. The idempotency key from 1.13 is exactly what makes replay safe.
- Clear UI for offline state, pending-sync count, and conflict resolution when a queued sale hits insufficient stock.
- Installable to home screen with a proper manifest and icons.

#### 4.3 Shift and cash drawer management

The gap between "sales recorded" and "cash in the drawer" is where stores actually lose money, and where a helper's accountability lives.

- Open a shift with a starting cash count; close it with a counted total, expected total, and variance.
- Record cash in and cash out mid-shift: paying a supplier on delivery, giving change float, the owner taking cash.
- Per-cashier attribution, so a variance belongs to a person and a shift.
- Shift history with variance trend. A store owner reads this screen every single day.

#### 4.4 E-load, GCash, and bill payment as service items

A huge share of real sari-sari revenue is not goods. Load, GCash cash-in and cash-out, bill payment, and padala all earn a service fee on a float balance rather than on inventory.

- A non-inventory service product type with a commission or fee rule.
- A **float balance** per service (your GCash wallet, your load wallet) that behaves like stock: it depletes, needs topping up, and reconciles at end of shift.
- Fee calculation by tier, since cash-in fees are banded by amount.
- Separate the service revenue from goods revenue in reporting, because the margins are nothing alike.

This one is unexpected and immediately signals domain research.

#### 4.5 Real-time sync across devices (SignalR)

Owner on a laptop in the back, helper on a phone at the counter, same store. Without this, the two screens silently disagree about stock.

- SignalR hub scoped per store (which the Phase 2 `StoreId` makes clean).
- Push stock changes, new transactions, and dashboard tiles.
- Optimistic UI with reconciliation on the authoritative event.

Technically impressive, genuinely useful, and a natural showcase for the tenancy work.

### Tier B: inventory and money depth

#### 4.6 Cost price and profit tracking

`Product` has `Price` but no cost, so the app reports revenue and can never report profit, which is the number owners actually care about.

- `CostPrice`, margin per product, daily and monthly profit on the dashboard.
- Weighted average cost, updated on each stock-in, so margin survives supplier price changes.
- "Most profitable" alongside the existing "top selling". They are usually different lists, which makes a genuinely interesting dashboard insight.

#### 4.7 Stock movement ledger

Stock changes silently inside checkout today. Add a `StockMovement` table (sale, stock-in, repack, adjustment, spoilage, personal consumption, count correction) with reason, user, and timestamp.

This gives a real audit trail, makes "why is my count wrong" answerable, and turns `StockQuantity` into a derivable value you can reconcile against. It is also the honest foundation for 4.1, 4.3, and 4.9.

Include **personal consumption** explicitly. Owners take goods for the family constantly, and untracked it is the single largest source of inventory drift in a real store.

#### 4.8 Supplier management and reorder intelligence

- Suppliers, purchase orders, and delivery receiving against a PO (partial deliveries included).
- Per-supplier price history, so you can show the same item cheaper elsewhere.
- **Sales velocity and days-of-stock-remaining**: "Coke 1.5L runs out in 3 days at current pace."
- Reorder point per product derived from velocity and lead time, and a generated suggested order list.
- Delivery day scheduling, since most sari-sari stores reorder on fixed supplier routes.

#### 4.9 Expiry tracking and FEFO

Perishables and near-dated stock are real losses.

- Batch or lot per stock-in with expiry date.
- First-expired-first-out picking hint at checkout.
- Expiry dashboard: expiring in 7 days, 30 days, already expired.
- Markdown workflow: discount near-dated stock rather than writing it off, and report what the markdown recovered.

#### 4.10 Dead stock and capital report

Items with no sale in 30, 60, or 90 days, with the peso value of capital tied up in them. For a store running on thin working capital this is the most actionable report in the app, and it is a simple query over data you already have.

#### 4.11 Utang management, properly

`StoreSettings.SmsAlertTemplate` exists, seeded with "Hi Suki, paalala lang...", and nothing uses it.

- Wire up reminders: render the template, then send via the Web Share API, an SMS link, or a Messenger link. No paid gateway needed.
- Credit limit per customer, with a warning at checkout and an override that gets logged.
- Aging buckets: 0 to 7 days, 8 to 30, over 30.
- Payment history and a running statement per customer, printable or shareable as an image.
- Partial payments against specific transactions rather than a single lump balance.
- Store credit the other direction: `paunang bayad`, where a customer prepays and draws down.

#### 4.12 Void, refund, and price override audit

A sale, once recorded, can never be reversed, and real stores mis-ring constantly.

- Void with a required reason that reverses stock and credit, recorded rather than deleted.
- Partial refund by line item.
- Every price override and void logged with user and reason, visible to the owner.

### Tier C: reporting and operations

#### 4.13 End-of-day Z-report

Daily reconciliation: expected versus counted cash, variance, sales by payment type, service revenue, utang extended, payments collected, top items. Printable. This is the feature that signals you talked to an actual store owner.

#### 4.14 Basket analysis

Which items get bought together, from `TransactionItem` data you already store. Suggests bundles and shelf placement. A simple co-occurrence count is enough to look smart, and it is a nice dashboard widget.

#### 4.15 Bundles, promos, and price tiers

- Combo items ("pang-almusal" bundle) priced below the sum of parts, which decrement component stock correctly.
- Retail versus `bultuhan` wholesale price per product, with a quantity break that switches tier automatically.
- Time-boxed promos with start and end dates.
- Suki loyalty: points or a regular-customer discount tier.

#### 4.16 Reports and export

Date-range sales, CSV and Excel export, PDF receipts, and printable 58mm thermal receipt CSS building on the existing `ReceiptModalComponent`. Scheduled daily digest by email or Messenger.

#### 4.17 Multi-branch

Owners who do well open a second store. With Phase 2 tenancy in place, a store group with consolidated reporting and per-branch stock is a modest extension that greatly widens what the app claims to do.

### Tier D: onboarding and usability

#### 4.18 Starter catalog and CSV import

The highest-friction moment is an empty inventory. Nobody types 200 products by hand.

- A pre-loaded catalog of common Philippine sari-sari products with barcodes, categories, and typical prices, which the owner picks from and adjusts.
- CSV and Excel import with a column mapping step, a validation preview, and partial-failure reporting.
- A first-run wizard: store name, timezone, currency, low-stock threshold, then catalog or import.

This converts the demo from "look at my seeded data" to "add your own store in two minutes".

#### 4.19 Barcode scanning

`@zxing/ngx-scanner` with the device camera, for checkout and stock-in, plus a `Barcode` field on `Product`. Visually impressive in a demo video and roughly a day of work.

#### 4.20 Speed-first checkout

A cashier should never need the mouse.

- Numpad-first entry, quantity shortcuts, keyboard-only flow.
- Global command palette (`Ctrl+K`) for jumping to a product, customer, or page.
- Held or parked sales, so a customer who forgot their wallet does not block the queue.
- Favorite and most-sold tiles pinned, which is roughly what `GetQuickItemsAsync` is reaching for already.

#### 4.21 Roles: owner and helper

A tindera should sell and extend utang but not change prices, see margins, or void transactions. Identity roles plus route guards plus server-side authorization policies. Cheap, and it shows you think about authorization and not only authentication.

#### 4.22 Price tags and shelf labels

Printable price tags and shelf labels, with barcodes if 4.19 lands. Small, practical, and a detail that reads as having thought past the screen.

#### 4.23 Notifications and activity feed

Low-stock alerts, expiry warnings, overdue utang, shift variances, all in one notification center, with an owner-facing activity feed of what the helper did today.

#### 4.24 Polish

- Dark mode.
- Tagalog and English toggle, with the API strings localized to match (1.5).
- Accessibility pass: focus management, ARIA on modals, contrast. Lighthouse scores in the README.
- Data export and account deletion, which reads as professional maturity.

### Tier E: engineering showcase

Features aimed at the reviewer rather than the store owner.

- **Background jobs** with Hangfire, which you already use at DataHub: nightly demo reset, low-stock digest, utang reminders, dead-stock recompute, velocity recalculation.
- **OpenTelemetry** traces and metrics, so you can show a real trace through checkout.
- **Output caching** on the dashboard, invalidated on write, with the cache strategy explained in the README.
- **API versioning** and consistent pagination metadata.
- **An EF `SaveChangesAsync` audit interceptor** feeding the audit log, so auditing is structural rather than sprinkled.
- **Feature flags**, so half-built features can ship dark.
- **Architecture tests** (NetArchTest) asserting Domain references nothing and Application never references Infrastructure. This proves the Clean Architecture claim instead of asserting it.

---

## Phase 5: Make it readable as a portfolio piece

Smallest effort, largest return, and currently the weakest part of the repo.

### 5.1 Replace the README

`frontend/talaan-ni-ana/README.md` is still untouched Angular CLI boilerplate and the repo root has no README at all. For most reviewers the README is the project. Write one at the root with:

- One line on what it is and who it serves.
- **The live demo link and demo credentials, above everything else.**
- A GIF or 60-second video of a sale being rung up, including the offline flow if 4.2 lands.
- Screenshots of dashboard, checkout, utang detail.
- Stack, plus an architecture diagram showing the four-project dependency direction.
- A **design decisions and tradeoffs** section: why Clean Architecture at this size, why a transaction around checkout, why global query filters for tenancy, why the denormalized credit balance and how you keep it honest, how tingi conversion is modeled, what you deliberately left out. This is what separates a junior README from a senior one.
- A **known limitations** section. Naming your own gaps reads as confidence.
- Local setup that actually works from a clean clone.

### 5.2 Tests

`angular.json` sets `skipTests: true` on every schematic and there is not one test file in either half. For a portfolio project this is the most common and most fatal gap.

You do not need high coverage. You need tests that prove you know what is worth testing:

- **Unit:** `CheckoutService` (insufficient stock, empty cart, credit without a customer, server-side price enforcement, rollback on failure) and the tingi conversion math from 4.1.
- **Integration:** the API against Postgres via Testcontainers, covering the auth flow and the cross-tenant isolation test from Phase 2.
- **Frontend:** a few component tests on cart math and the peso pipe.
- **E2E:** one Playwright happy path, login to sale to receipt, in CI.

Then put the coverage badge in the README.

### 5.3 API documentation

Swagger is dev-only. Enable it in production behind a simple gate, or publish the OpenAPI spec, and link it from the README. Add XML comments and response type attributes so the generated docs say something.

### 5.4 Upgrade the frontend

Angular 18 against .NET 10 is a mismatched signal. Move to the current Angular major, drop `zone.js` for zoneless change detection (the codebase already uses signals, so this is a short step), and adopt the new control flow syntax wherever `*ngIf` and `*ngFor` remain. "Zoneless Angular with signals" is a line worth having in the README.

### 5.5 Repository hygiene

- Root README, LICENSE, CONTRIBUTING, CHANGELOG tied to your phase tags.
- `.editorconfig` for the backend to match the frontend's.
- Verify `backend/.vs/` and all `bin/` and `obj/` output are ignored; they are present on disk now.
- `global.json` to pin the SDK.
- `docker-compose.yml` bringing up Postgres, API, and frontend in one command. Reviewers who can run your project in 30 seconds think better of it.

---

## Suggested sequence

| Phase | Work | Rough effort | Why this order |
|---|---|---|---|
| 1 | Correctness and security | ~1 week | No point deploying a 1-centavo exploit |
| 2 | Multi-tenancy | ~1 week | Blocks any public URL where strangers can register |
| 3 | Postgres + Render + Cloudflare + demo mode | ~3 to 4 days | A live link is what makes the project visible at all |
| 5.1 | README, screenshots, demo GIF | ~1 day | Highest value per hour in the plan |
| 5.2 | Tests + CI | ~3 to 4 days | The most common portfolio gap; closing it stands out |
| 4.7 | Stock movement ledger | ~3 days | Foundation for tingi, shifts, expiry, spoilage |
| 4.2 | Offline PWA | ~1 week | Standout feature, and right for the domain |
| 4.1 | Tingi and repacking | ~4 to 5 days | The most domain-authentic thing you can build |
| 4.6, 4.13, 4.19 | Profit, Z-report, barcode | ~1 week | Visible, demoable, credible |
| 4.3, 4.4 | Shifts and cash drawer, e-load and GCash | ~1.5 weeks | Unexpected depth; signals real research |
| 4.18 | Starter catalog and CSV import | ~3 days | Turns the demo into something usable |
| 4.5 | Real-time sync | ~4 days | Engineering showcase on top of tenancy |
| 4.8 to 4.12, 4.21 | Suppliers, expiry, dead stock, utang, void, roles | ~3 weeks | Depth, if you keep going |
| Tier E, 5.3 to 5.5 | Observability, docs, Angular upgrade, hygiene | ~1 week | Finishing pass |

**Minimum for "deployed and professional":** Phases 1, 2, 3, 5.1, 5.2, plus 4.7 and 4.2. Roughly six weeks of evenings, leaving a live, offline-capable, tested, multi-tenant POS with a README that explains itself.

**If you want one feature that makes people remember it:** 4.1 tingi and repacking, or 4.4 e-load and GCash float. Both are invisible in every POS tutorial and obvious to anyone who has actually stood in a sari-sari store.

---

## Quick wins (an afternoon each)

- Swap `alert()` in `error.interceptor` for the `ToastService` that is already written and unused.
- Delete the stray `using System.Transactions;` from `Customer.cs`; rename `Products.cs` to `Product.cs`.
- Move the JWT key to user-secrets and add startup validation.
- Add `Take(n)` to `GetQuickItemsAsync`.
- Replace `pageSize: int.MaxValue` with a `SumAsync`.
- Map checkout's insufficient-stock exception to a 409 so the cashier sees the real message.
- Write the root README skeleton, even before there is a live link to put in it.
