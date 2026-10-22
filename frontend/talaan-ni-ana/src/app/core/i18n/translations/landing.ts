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

  'landing.closeTitle': { tl: 'Tingnan mo na', en: 'Take a look' },
  'landing.madeBy': { tl: 'Gawa ni', en: 'Made by' }
};
