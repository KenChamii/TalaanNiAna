/**
 * Every string the app shows twice.
 *
 * Keyed by where it appears rather than by its English text, so changing the
 * wording of one screen cannot silently change another that happened to say
 * the same thing.
 *
 * Tagalog first, because that is the default and the one written from scratch;
 * the English is the translation, not the other way round.
 */
export const DICTIONARY: Record<string, { tl: string; en: string }> = {
  // ---- Navigation and chrome ----
  'nav.menu': { tl: 'Menu', en: 'Menu' },
  'nav.dashboard': { tl: 'Dashboard', en: 'Dashboard' },
  'nav.checkout': { tl: 'Benta', en: 'Sell' },
  'nav.inventory': { tl: 'Imbentaryo', en: 'Inventory' },
  'nav.customers': { tl: 'Suki', en: 'Customers' },
  'nav.transactions': { tl: 'Transaksyon', en: 'Transactions' },
  'nav.zreport': { tl: 'Z-Report', en: 'Z-Report' },
  'nav.shrinkage': { tl: 'Lugi sa Sira', en: 'Losses' },
  'nav.settings': { tl: 'Settings', en: 'Settings' },
  'nav.logout': { tl: 'Mag-logout', en: 'Log out' },
  'nav.noStore': { tl: 'Walang pangalan', en: 'Unnamed store' },

  'theme.toDark': { tl: 'Gabi', en: 'Dark' },
  'theme.toLight': { tl: 'Araw', en: 'Light' },
  'lang.toEnglish': { tl: 'English', en: 'English' },
  'lang.toTagalog': { tl: 'Tagalog', en: 'Tagalog' },

  // ---- Shared actions ----
  'action.save': { tl: 'I-save', en: 'Save' },
  'action.saving': { tl: 'Sine-save…', en: 'Saving…' },
  'action.cancel': { tl: 'Kanselahin', en: 'Cancel' },
  'action.delete': { tl: 'Tanggalin', en: 'Delete' },
  'action.edit': { tl: 'Edit', en: 'Edit' },
  'action.print': { tl: 'I-print', en: 'Print' },
  'action.search': { tl: 'Maghanap', en: 'Search' },
  'action.loading': { tl: 'Naglo-load…', en: 'Loading…' },

  // ---- Landing ----
  'landing.eyebrow': { tl: 'Point of sale para sa sari-sari store', en: 'Point of sale for the corner store' },
  'landing.title': { tl: 'Alam mo\nang tubo mo', en: 'Know what\nyou actually made' },
  'landing.copy': {
    tl: 'Karamihan sa sari-sari store ay naka-notebook. Naitatala ang naibenta pero hindi ang puhunan, kaya alam ng may-ari na tatlong libo ang pumasok at wala siyang ideya kung magkano doon ang kanya. Ito ang notebook, maayos na itinala.',
    en: 'Most corner stores run on a notebook. It records what was sold but never what it cost, so the owner knows the day took three thousand pesos and has no idea how much of that was theirs. This is the notebook, kept properly.'
  },
  'landing.demo': { tl: 'Subukan ang demo', en: 'Try the demo' },
  'landing.demoOpening': { tl: 'Binubuksan…', en: 'Opening…' },
  'landing.code': { tl: 'Basahin ang code', en: 'Read the code' },
  'landing.demoNote': {
    tl: 'Totoong tindahan na may dalawang linggong benta. Walang sign-up, walang i-install. Natutulog ang API kapag walang gumagamit, kaya maaaring umabot ng isang minuto ang unang bukas.',
    en: 'A real store with two weeks of sales. No sign-up, nothing to install. The API sleeps when idle, so the first load can take a minute.'
  },
  'landing.demoFailed': { tl: 'Hindi available ang demo sa ngayon.', en: 'The demo is not available right now.' },
  'landing.featuresEyebrow': { tl: 'Ano ang kaya nito', en: 'What it does' },
  'landing.featuresTitle': { tl: 'Siyam na bagay na hindi kaya ng notebook', en: 'Nine things a notebook cannot do' },
  'landing.buildEyebrow': { tl: 'Paano ginawa', en: 'How it is built' },
  'landing.buildTitle': { tl: 'Ang pagkakagawa', en: 'The build' },
  'landing.nextEyebrow': { tl: 'Susunod', en: 'Next' },
  'landing.nextTitle': { tl: 'Ano ang gawa na, at ano ang hindi pa', en: 'What is built, and what is not' },
  'landing.closeTitle': { tl: 'Tingnan mo na', en: 'Take a look' },

  // ---- Auth ----
  'auth.login': { tl: 'Mag-login', en: 'Log in' },
  'auth.welcomeBack': { tl: 'Maligayang pagbabalik', en: 'Welcome back' },
  'auth.username': { tl: 'Pangalan / Email', en: 'Username / Email' },
  'auth.password': { tl: 'Password', en: 'Password' },
  'auth.remember': { tl: 'Tandaan ako', en: 'Remember me' },
  'auth.enter': { tl: 'Pumasok', en: 'Sign in' },
  'auth.noAccount': { tl: 'Wala ka pang account?', en: 'No account yet?' },
  'auth.createHere': { tl: 'Gumawa dito', en: 'Create one' },
  'auth.createAccount': { tl: 'Gumawa ng account', en: 'Create account' },
  'auth.or': { tl: 'o', en: 'or' },
  'auth.badCredentials': { tl: 'Mali ang pangalan/email o password.', en: 'That username or password is wrong.' },

  // ---- Till ----
  'pos.eyebrow': { tl: 'Bentahan', en: 'Selling' },
  'pos.title': { tl: 'Talaan ni Ana POS', en: 'Talaan ni Ana POS' },
  'pos.total': { tl: 'Kabuuan', en: 'Total' },
  'pos.pieces': { tl: 'piraso', en: 'pieces' },
  'pos.scanPlaceholder': { tl: 'I-scan ang barcode, o i-type ang code at Enter', en: 'Scan a barcode, or type a code and press Enter' },
  'pos.quickEyebrow': { tl: 'Isang tap lang', en: 'One tap' },
  'pos.quickTitle': { tl: 'Mabilisang Pindutan', en: 'Quick buttons' },
  'pos.searchPlaceholder': { tl: 'Hanapin ang paninda…', en: 'Find an item…' },
  'pos.noMatch': { tl: 'Walang tugmang paninda.', en: 'Nothing matches.' },
  'pos.cartEyebrow': { tl: 'Kasalukuyang kareta', en: 'Current cart' },
  'pos.cartTitle': { tl: 'Ang Resibo', en: 'The receipt' },
  'pos.clear': { tl: 'Linisin', en: 'Clear' },
  'pos.emptyCart': { tl: 'Walang item sa kart pa.', en: 'Nothing in the cart yet.' },
  'pos.emptyHint': { tl: 'I-scan o i-tap ang paninda.', en: 'Scan or tap an item.' },
  'pos.each': { tl: 'bawat isa', en: 'each' },
  'pos.payable': { tl: 'Bayaran', en: 'To pay' },
  'pos.paymentLabel': { tl: 'Paraan ng pagbayad', en: 'Payment' },
  'pos.cash': { tl: 'Cash', en: 'Cash' },
  'pos.cashHint': { tl: 'Abot-kamay', en: 'Paid now' },
  'pos.credit': { tl: 'Utang', en: 'Credit' },
  'pos.creditHint': { tl: 'Listahan', en: 'On the list' },
  'pos.pickCustomer': { tl: 'Piliin ang suki', en: 'Choose a customer' },
  'pos.pickCustomerOption': { tl: '-- Pumili ng suki --', en: '-- Choose a customer --' },
  'pos.checkout': { tl: 'Irehistro ang Benta', en: 'Record the sale' },
  'pos.processing': { tl: 'Pinoproseso…', en: 'Working…' },

  // ---- Offline ----
  'offline.noConnection': { tl: 'Walang koneksyon. Tuloy lang ang benta.', en: 'No connection. Keep selling.' },
  'offline.sending': { tl: 'Ipinapadala ang naunang benta…', en: 'Sending earlier sales…' },
  'offline.waiting': { tl: 'naghihintay', en: 'waiting' },
  'offline.retry': { tl: 'Subukan ulit ngayon', en: 'Try again now' },
  'offline.discard': { tl: 'Burahin', en: 'Discard' },
  'offline.queued': { tl: 'Nakatala muna rito. Ipapadala kapag may signal.', en: 'Held here. It will send when there is signal.' },
  'offline.nothingWaiting': {
    tl: 'Walang naghihintay na benta. Ang susunod ay itatago muna rito hanggang bumalik ang internet.',
    en: 'Nothing waiting. The next sale will be held here until the connection returns.'
  }
};
