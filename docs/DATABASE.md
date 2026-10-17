# Database

PostgreSQL 17. The schema is owned by EF Core migrations; nothing is applied by hand.

## Running it locally

```
docker compose -f docker-compose.dev.yml up -d
docker compose -f docker-compose.dev.yml ps
```

The `db` service must read `healthy`, not just `Up`, before migrations will
connect. Data survives `down` and is wiped by `down -v`.

The host port is **5433**, not 5432. A machine with PostgreSQL installed
natively already holds 5432, and both can bind without error, so the clash
surfaces later as a password authentication failure against the wrong server.

## Applying migrations

```
cd backend/TalaanNiAna.Infrastructure
dotnet ef migrations add <Name> --startup-project ../TalaanNiAna.API
dotnet ef database update --startup-project ../TalaanNiAna.API
```

## Things that are easy to get wrong

**Timezone.** The connection string pins `Timezone=UTC` and the container sets
`TZ=UTC`. The daily sales chart groups by local date using `date_trunc`, which
reads the session timezone, and the store's own offset is applied in the query.
If the session timezone stops being UTC, that chart shifts days without raising
an error. Both ends have to agree.

**Case sensitivity.** Postgres compares strings case-sensitively. Product and
customer search go through `ILIKE`, not `Contains`. A new search written with
`Contains` will look correct in testing and quietly miss rows in use.

**Concurrency.** `Product` has no `RowVersion` column. Postgres keeps a row
version in its own `xmin` system column, declared as a shadow property in
`AppDbContext`. Checkout still relies on `DbUpdateConcurrencyException` to catch
two sales taking the same stock.

**Wildcards in search terms.** `%`, `_` and `\` typed by a user are escaped by
`SearchPattern` before reaching `ILIKE`, so a customer named "100% Lucky" is a
literal match rather than a wildcard.

**Changing the credentials.** `POSTGRES_USER` and `POSTGRES_PASSWORD` are read
only when the volume is first initialised. Editing them later has no effect
until `docker compose -f docker-compose.dev.yml down -v` wipes the volume.
