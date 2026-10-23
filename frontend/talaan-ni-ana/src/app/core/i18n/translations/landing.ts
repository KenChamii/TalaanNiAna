import { Translations } from '../translation.model';

export const LANDING: Translations = {
  'landing.source': { tl: 'Source', en: 'Source' },
  'landing.eyebrow': { tl: 'Point of sale para sa sari-sari store', en: 'Point of sale for the sari-sari store' },
  'landing.title': { tl: 'Alam mo\nang tubo mo', en: 'Know what\nyou really earn' },
  'landing.copy': {
    tl: 'Karamihan sa sari-sari store ay sa notebook pa rin nagtatala. Nakasulat doon ang naibenta, pero hindi ang puhunan. Kaya sa pagsasara, alam ng may-ari na ₱3,000 ang laman ng kaha, pero hindi kung magkano roon ang tubo. Ganoon pa rin ang talaan sa Talaan ni Ana, pero kasama na ang puhunan.',
    en: 'Most sari-sari stores still keep their records in a notebook. It shows what was sold, but not what each item cost. At closing, the owner knows the drawer holds ₱3,000 but not how much of it is profit. Talaan ni Ana keeps the same records, with the cost side filled in.'
  },
  'landing.demo': { tl: 'Subukan ang demo', en: 'Try the demo' },
  'landing.demoOpening': { tl: 'Binubuksan…', en: 'Opening…' },
  'landing.code': { tl: 'Basahin ang code', en: 'Read the code' },
  'landing.demoNote': {
    tl: 'Magbubukas ito ng sample na tindahan na may dalawang linggong benta. Hindi kailangang mag-sign up. Natutulog ang server kapag walang gumagamit, kaya maaaring umabot nang isang minuto ang unang pagbukas.',
    en: 'Opens a sample store with two weeks of sales. No sign-up needed. The server sleeps when idle, so the first load may take up to a minute.'
  },
  'landing.demoFailed': { tl: 'Hindi mabuksan ang demo sa ngayon.', en: 'The demo isn’t available right now.' },
  'landing.demoFailedLink': { tl: 'Gumawa na lang ng account para masubukan ang app.', en: 'Create an account to try the app instead.' },

  'landing.sample.caption': { tl: 'Halimbawa ng isang araw', en: 'A sample day' },
  'landing.sample.item': { tl: 'Paninda', en: 'Item' },
  'landing.sample.quantity': { tl: 'Dami', en: 'Qty' },
  'landing.sample.sales': { tl: 'Benta', en: 'Sales' },
  'landing.sample.cost': { tl: 'Puhunan', en: 'Cost' },
  'landing.sample.profit': { tl: 'Tubo', en: 'Profit' },

  'landing.featuresEyebrow': { tl: 'Ano ang kaya nito', en: 'What it does' },
  'landing.featuresTitle': { tl: 'Ang hindi kaya ng notebook', en: 'What a notebook can’t do' },
  'landing.howItWorks': { tl: 'Paano ito gumagana', en: 'How it works' },

  'landing.feature.checkout.tag': { tl: 'Benta', en: 'Checkout' },
  'landing.feature.checkout.title': { tl: 'Mabilis na bentahan', en: 'Fast checkout' },
  'landing.feature.checkout.body': {
    tl: 'I-scan o i-tap ang paninda, ayusin ang dami, at tanggapin ang bayad o ilista bilang utang.',
    en: 'Scan or tap items, adjust quantities, and take cash or record the sale as utang.'
  },
  'landing.feature.checkout.detail': {
    tl: 'Gumagana sa kahit anong USB barcode scanner, dahil parang keyboard lang ang scanner. Laging nakatutok ang scan field, at naidadagdag ang item kapag pinindot ang Enter. Kapag walang scanner, puwedeng i-type ang code.',
    en: 'Works with any USB barcode scanner, since scanners type like a keyboard. The scan field keeps focus and Enter adds the item. Without a scanner, the code can be typed in by hand.'
  },

  'landing.feature.profit.tag': { tl: 'Puhunan', en: 'Cost' },
  'landing.feature.profit.title': { tl: 'Puhunan at totoong tubo', en: 'Cost price and actual profit' },
  'landing.feature.profit.body': {
    tl: 'Nakatala ang puhunan ng bawat paninda, kaya kalkulado ang tubo, hindi hula.',
    en: 'Every item stores what it cost, so profit is calculated, not estimated.'
  },
  'landing.feature.profit.detail': {
    tl: 'Sine-save ang puhunan sa bawat linya ng benta sa oras ng pagbili. Kapag nagtaas ng presyo ang supplier sa susunod na linggo, hindi magbabago ang tubo ng mga naunang benta. Puwedeng magbenta nang mas mababa sa puhunan, at lalabas itong lugi.',
    en: 'The cost is saved on each sale line at the time of sale. If a supplier raises prices next week, the profit on past sales stays the same. Items sold below cost are allowed and show up as a loss.'
  },

  'landing.feature.credit.tag': { tl: 'Utang', en: 'Credit' },
  'landing.feature.credit.title': { tl: 'Maayos na listahan ng utang', en: 'A credit list that adds up' },
  'landing.feature.credit.body': {
    tl: 'Makikita kung sino ang may utang, sino ang nagbayad ngayon, at ang kabuuang hindi pa bayad.',
    en: 'See who owes what, who paid today, and the total still outstanding.'
  },
  'landing.feature.credit.detail': {
    tl: 'Kinukuwenta ang balanse mula sa buong kasaysayan ng bayad, kaya laging tugma ang total sa mga transaksyon sa likod nito.',
    en: 'Balances are recalculated from the full payment history, so the total always matches the transactions behind it.'
  },

  'landing.feature.closing.tag': { tl: 'Pagsasara', en: 'Closing' },
  'landing.feature.closing.title': { tl: 'Z-report sa pagsasara', en: 'End-of-day Z-report' },
  'landing.feature.closing.body': {
    tl: 'Kung magkano dapat ang laman ng kaha sa pagsasara, at kung kumita ba ang araw.',
    en: 'How much should be in the drawer at closing, and whether the day made money.'
  },
  'landing.feature.closing.detail': {
    tl: 'Binibilang ang cash na benta at ang bayad sa utang. Hindi kasama ang inutang na benta dahil walang perang pumasok. Puwedeng i-print ulit ang report ng kahit anong nakaraang araw.',
    en: 'Counts cash sales and utang payments, and leaves out credit sales since no cash changed hands. Reports for past days can be reprinted anytime.'
  },

  'landing.feature.inventory.tag': { tl: 'Imbentaryo', en: 'Inventory' },
  'landing.feature.inventory.title': { tl: 'Babala kapag paubos na', en: 'Low-stock alerts' },
  'landing.feature.inventory.body': {
    tl: 'Magtakda ng minimum na stock sa bawat paninda, tingnan ang margin ng bawat isa, at maghanap gamit ang pangalan o barcode.',
    en: 'Set a reorder level for each item, see the margin on every product, and search by name or barcode.'
  },
  'landing.feature.inventory.detail': {
    tl: 'Sa checkout, sa database kinukuha ang presyo at stock, hindi sa browser. May concurrency control din para hindi sabay na maibenta ng dalawang register ang huling piraso.',
    en: 'At checkout, prices and stock come from the database, not the browser. Concurrency control stops two registers from selling the last item at the same time.'
  },

  'landing.feature.ledger.tag': { tl: 'Talaan', en: 'Stock log' },
  'landing.feature.ledger.title': { tl: 'Kasaysayan ng stock', en: 'Stock history with reasons' },
  'landing.feature.ledger.body': {
    tl: 'Nakatala ang bawat delivery, pagbilang, sira at return.',
    en: 'Every delivery, recount, spoilage and return is recorded.'
  },
  'landing.feature.ledger.detail': {
    tl: 'Sine-save ng bawat entry ang balanse sa oras na iyon, kaya hindi nababago ng mga susunod na pagwawasto ang lumang talaan. Kapag kulang ang bilang sa stock-take, matutunton kung saan ito napunta.',
    en: 'Each entry keeps the balance at that moment, so later corrections never change past records. When a stock count comes up short, you can trace where it went.'
  },

  'landing.feature.tingi.tag': { tl: 'Tingi', en: 'By the piece' },
  'landing.feature.tingi.title': { tl: 'Bentang tingi', en: 'Sell by the piece' },
  'landing.feature.tingi.body': {
    tl: 'Magbukas ng isang kahon at ibenta nang paisa-isa ang laman, nang tama ang bilang ng kahon at ng piraso.',
    en: 'Open a box and sell its contents one at a time, with both counts kept accurate.'
  },
  'landing.feature.tingi.detail': {
    tl: 'Sa iisang transaksyon, ibinabawas ang kahon at idinadagdag ang mga piraso. Ang puhunan bawat piraso ay ang halaga ng kahon na hinati sa dami ng laman nito, at kinukuwenta ulit sa bawat repack dahil nagbabago ang presyo ng supplier.',
    en: 'Repacking removes the box and adds the pieces in a single transaction. The cost per piece is the box cost divided by how many pieces it holds, recalculated on every repack since supplier prices change.'
  },

  'landing.feature.offline.tag': { tl: 'Walang signal', en: 'Offline' },
  'landing.feature.offline.title': { tl: 'Tuloy ang benta kahit walang signal', en: 'Works offline' },
  'landing.feature.offline.body': {
    tl: 'Sa device muna nase-save ang benta, at ipinapadala ito kapag may koneksyon na.',
    en: 'Sales are saved on the device and sent once the connection is back.'
  },
  'landing.feature.offline.detail': {
    tl: 'May idempotency key ang bawat checkout, kaya hindi madodoble ang bentang naitala na ng server kapag nag-sync. Ang benta lang na hindi umabot sa server ang naghihintay; agad na lumalabas ang mga tinanggihan.',
    en: 'Each checkout carries an idempotency key, so a sale the server already recorded is never counted twice when it syncs. Only sales that couldn’t reach the server are queued; rejected ones show up right away.'
  },

  'landing.feature.stores.tag': { tl: 'Maraming tindahan', en: 'Multi-store' },
  'landing.feature.stores.title': { tl: 'Para sa maraming tindahan', en: 'Multi-store support' },
  'landing.feature.stores.body': {
    tl: 'Sariling paninda, suki at benta lang ang nakikita ng bawat tindahan.',
    en: 'Each store sees only its own products, customers and sales.'
  },
  'landing.feature.stores.detail': {
    tl: 'Ipinapatupad ito ng global query filters sa data layer, kaya awtomatikong naka-scope sa tindahan ang bawat bagong query.',
    en: 'Store isolation is enforced by global query filters in the data layer, so every new query is scoped to the store by default.'
  },

  'landing.buildEyebrow': { tl: 'Paano ginawa', en: 'How it’s built' },
  'landing.buildTitle': { tl: 'Sa likod ng app', en: 'Under the hood' },

  'landing.decision.idempotency.title': { tl: 'Walang dobleng singil', en: 'No double charges' },
  'landing.decision.idempotency.body': {
    tl: 'May idempotency key ang bawat checkout na natatangi sa bawat tindahan. Kapag na-double tap, naputol ang koneksyon o inulit ang request, ibinabalik ang orihinal na benta sa halip na gumawa ng bago.',
    en: 'Every checkout has an idempotency key, unique per store. A double tap, a dropped connection or a retry returns the original sale instead of creating a new one.'
  },
  'landing.decision.timezone.title': { tl: 'Lokal na oras ang batayan ng araw', en: 'Sales days follow local time' },
  'landing.decision.timezone.body': {
    tl: 'Timezone ng tindahan ang ginagamit sa simula at dulo ng araw, saka ito kino-convert sa UTC. Kaya ang benta nang 9 PM sa Manila ay sa araw na iyon nabibilang, hindi sa kinabukasan.',
    en: 'Day boundaries use the store’s own timezone and are converted to UTC, so a 9 PM sale in Manila counts toward that day, not the next.'
  },
  'landing.decision.pricing.title': { tl: 'Server ang nagtatakda ng presyo', en: 'The server sets the price' },
  'landing.decision.pricing.body': {
    tl: 'Ipinapadala ng register ang presyong inaasahan nito. Ikinukumpara ito ng server sa database, at tinatanggihan ang benta kapag hindi tugma.',
    en: 'The register sends the price it expects. The server checks it against the database and rejects the sale if the two don’t match.'
  },
  'landing.decision.errors.title': { tl: 'Malinaw na error message', en: 'Clear error messages' },
  'landing.decision.errors.body': {
    tl: 'Sumusunod sa RFC 7807 ang mga error at may stable na error code, kaya tiyak na mensahe ang nakikita ng cashier, gaya ng "kulang ang stock ng Coke", hindi generic na server error.',
    en: 'Errors follow RFC 7807 with a stable error code, so the cashier sees a specific message like "kulang ang stock ng Coke" instead of a generic server error.'
  },

  'landing.archEyebrow': { tl: 'Arkitektura', en: 'Architecture' },
  'landing.archTitle': { tl: 'Ganito dumadaloy ang isang benta', en: 'How a sale travels' },
  'landing.archBody': {
    tl: 'Apat na proyekto sa backend, at laging papasok ang mga dependency. Walang alam ang Application layer sa EF Core, kaya ang API middleware ang nagiging 409 sa concurrency error.',
    en: 'Four backend projects, with dependencies always pointing inward. The Application layer knows nothing about EF Core, so the API middleware is what turns a concurrency error into a 409.'
  },
  'landing.route.till': { tl: 'Register (Angular PWA)', en: 'Till (Angular PWA)' },
  'landing.route.outbox': { tl: 'Outbox (IndexedDB)', en: 'Outbox (IndexedDB)' },
  'landing.route.api': { tl: 'ASP.NET Core API', en: 'ASP.NET Core API' },
  'landing.route.app': { tl: 'Application services', en: 'Application services' },
  'landing.route.db': { tl: 'EF Core → PostgreSQL', en: 'EF Core → PostgreSQL' },
  'landing.routeNote': {
    tl: 'Kapag walang signal, sa Outbox muna papasok ang benta at ipapadala kapag may koneksyon na. Sa ilalim, may global query filter ang bawat tindahan, kaya hindi makikita ng isa ang data ng iba.',
    en: 'With no signal, a sale waits in the outbox and is sent when the connection returns. At the bottom, a global query filter scopes every query to the store, so one store never sees another’s data.'
  },

  'landing.layer.domain.name': { tl: 'Domain', en: 'Domain' },
  'landing.layer.domain.text': { tl: 'Mga entity at interface. Walang dependency.', en: 'Entities and interfaces. No dependencies.' },
  'landing.layer.application.name': { tl: 'Application', en: 'Application' },
  'landing.layer.application.text': { tl: 'Mga service, DTO at validator (FluentValidation).', en: 'Services, DTOs and validators (FluentValidation).' },
  'landing.layer.infrastructure.name': { tl: 'Infrastructure', en: 'Infrastructure' },
  'landing.layer.infrastructure.text': { tl: 'EF Core, mga repository, Identity at migration.', en: 'EF Core, repositories, Identity and migrations.' },
  'landing.layer.api.name': { tl: 'API', en: 'API' },
  'landing.layer.api.text': { tl: 'Mga controller, middleware, rate limit at composition.', en: 'Controllers, middleware, rate limits and composition.' },

  'landing.specTitle': { tl: 'Spec sheet', en: 'Spec sheet' },
  'landing.spec.backend.label': { tl: 'Backend', en: 'Backend' },
  'landing.spec.backend.value': { tl: '.NET 10, ASP.NET Core, clean architecture, EF Core 10', en: '.NET 10, ASP.NET Core, clean architecture, EF Core 10' },
  'landing.spec.frontend.label': { tl: 'Frontend', en: 'Frontend' },
  'landing.spec.frontend.value': { tl: 'Angular 18, standalone components, signals, installable PWA', en: 'Angular 18, standalone components, signals, installable PWA' },
  'landing.spec.database.label': { tl: 'Database', en: 'Database' },
  'landing.spec.database.value': { tl: 'PostgreSQL sa Neon (Singapore); xmin bilang concurrency token', en: 'PostgreSQL on Neon (Singapore); xmin as the concurrency token' },
  'landing.spec.auth.label': { tl: 'Auth', en: 'Auth' },
  'landing.spec.auth.value': { tl: 'JWT na maikli ang buhay, umiikot na refresh token (SHA-256 hash, httpOnly cookie)', en: 'Short-lived JWT, rotating refresh tokens (SHA-256 hashed, httpOnly cookie)' },
  'landing.spec.tenancy.label': { tl: 'Tenancy', en: 'Tenancy' },
  'landing.spec.tenancy.value': { tl: 'StoreId sa bawat entity, ipinapatupad ng global query filter', en: 'StoreId on every entity, enforced by a global query filter' },
  'landing.spec.errors.label': { tl: 'Errors', en: 'Errors' },
  'landing.spec.errors.value': { tl: 'RFC 7807 problem details na may stable code', en: 'RFC 7807 problem details with stable codes' },
  'landing.spec.offline.label': { tl: 'Offline', en: 'Offline' },
  'landing.spec.offline.value': { tl: 'IndexedDB outbox, ligtas i-replay dahil sa idempotency key', en: 'IndexedDB outbox, safe to replay thanks to idempotency keys' },
  'landing.spec.tests.label': { tl: 'Tests', en: 'Tests' },
  'landing.spec.tests.value': { tl: 'xUnit + FluentAssertions, hand-written na fakes sa halip na mocking library', en: 'xUnit + FluentAssertions, hand-written fakes instead of a mocking library' },
  'landing.spec.ci.label': { tl: 'CI/CD', en: 'CI/CD' },
  'landing.spec.ci.value': { tl: 'GitHub Actions: build, test, mga Docker image, deploy sa Cloudflare', en: 'GitHub Actions: build, test, Docker images, deploy to Cloudflare' },
  'landing.spec.hosting.label': { tl: 'Hosting', en: 'Hosting' },
  'landing.spec.hosting.value': { tl: 'Cloudflare Workers (frontend), Render (API), Neon (database)', en: 'Cloudflare Workers (frontend), Render (API), Neon (database)' },
  'landing.spec.languages.label': { tl: 'Wika', en: 'Languages' },
  'landing.spec.languages.value': { tl: 'Tagalog at English, may test na pumapalya kapag may kulang na salin', en: 'Tagalog and English, with a test that fails when a translation is missing' },

  'landing.limitsEyebrow': { tl: 'Mga limitasyon', en: 'Known limits' },
  'landing.limitsTitle': { tl: 'Ang hindi pa kaya nito', en: 'What it doesn’t do yet' },
  'landing.limitsBody': {
    tl: 'Itinatala ang mga ito nang hayag, hindi hinahayaang matuklasan na lang.',
    en: 'Recorded openly rather than left to be discovered.'
  },
  'landing.limit.1': { tl: 'Isang direksyon lang ang offline: nagpapadala ng benta, pero luma ang catalogue hangga’t hindi online.', en: 'Offline is one-way: sales queue and send, but the catalogue is only as fresh as the last time the app was online.' },
  'landing.limit.2': { tl: 'Isang antas lang ang repack: puwedeng buksan ang kahon, pero hindi na ang piraso.', en: 'Repacking is one level deep: a pack can be opened, a piece cannot be broken again.' },
  'landing.limit.3': { tl: 'Walang integration test sa totoong database. Mga fake ang gamit ng lahat ng test.', en: 'No integration tests against a real database. Every test runs against in-memory fakes.' },
  'landing.limit.4': { tl: 'Puwedeng baguhin ng kahit sino ang demo, kaya lumalayo ito sa orihinal na data.', en: 'The demo is writable by anyone, so it drifts from the seeded data.' },

  'landing.closeTitle': { tl: 'Tingnan mo na', en: 'Take a look' },
  'landing.madeBy': { tl: 'Gawa ni', en: 'Made by' }
};
