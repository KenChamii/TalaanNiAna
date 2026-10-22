import { Translations } from '../translation.model';

export const STOCK: Translations = {
  'stock.eyebrow': { tl: 'Galaw ng stock', en: 'Stock history' },
  'stock.remaining': { tl: 'Natitira ngayon', en: 'In stock now' },

  'stock.openPack': { tl: 'Magbukas ng pakete', en: 'Open a pack' },
  'stock.repackCopy': {
    tl: 'Ang isang {parent} ay may {units} na piraso nito. Hahatiin ang puhunan ng pakete sa bilang na iyon.',
    en: 'One {parent} holds {units} of these. The pack’s cost is divided by that number.'
  },
  'stock.packCount': { tl: 'Ilang pakete', en: 'Number of packs' },
  'stock.open': { tl: 'Buksan', en: 'Open' },
  'stock.opening': { tl: 'Binubuksan…', en: 'Opening…' },
  'stock.repacked': { tl: 'Nabuksan ang {packs} {parent}. +{pieces} {tingi}.', en: 'Opened {packs} × {parent}. +{pieces} {tingi}.' },

  'stock.record': { tl: 'Magtala ng galaw', en: 'Record a change' },
  'stock.actualCount': { tl: 'Aktwal na bilang', en: 'Actual count' },
  'stock.pieceCount': { tl: 'Ilang piraso', en: 'How many pieces' },
  'stock.note': { tl: 'Dahilan', en: 'Note' },
  'stock.notePlaceholder': { tl: 'hal. nabasag', en: 'e.g. broken' },
  'stock.preview': { tl: 'Magiging {count} ang natitira.', en: 'This leaves {count} in stock.' },
  'stock.save': { tl: 'Itala', en: 'Record' },
  'stock.saving': { tl: 'Itinatala…', en: 'Recording…' },
  'stock.noChange': { tl: 'Walang pagbabago sa stock.', en: 'That doesn’t change the stock.' },
  'stock.recorded': { tl: 'Naitala. {count} ang natitira.', en: 'Recorded. {count} left in stock.' },

  'stock.history': { tl: 'Kasaysayan', en: 'History' },
  'stock.transaction': { tl: 'Transaksyon #{id}', en: 'Transaction #{id}' },
  'stock.empty': { tl: 'Wala pang naitalang galaw.', en: 'No changes recorded yet.' },

  'stock.choice.Restock': { tl: 'Delivery', en: 'Delivery' },
  'stock.choice.Return': { tl: 'Balik', en: 'Return' },
  'stock.choice.Spoilage': { tl: 'Sira', en: 'Spoilage' },
  'stock.choice.Adjustment': { tl: 'Bilang', en: 'Recount' },

  'stock.hint.Restock': { tl: 'Dumating na paninda', en: 'New stock arrived' },
  'stock.hint.Return': { tl: 'Isinauli ng suki', en: 'Returned by a customer' },
  'stock.hint.Spoilage': { tl: 'Nasira, nag-expire, nabasag', en: 'Damaged, expired or broken' },
  'stock.hint.Adjustment': { tl: 'Hindi tugma ang bilang', en: 'The count doesn’t match' },

  'stock.reason.Opening': { tl: 'Simulang bilang', en: 'Opening count' },
  'stock.reason.Sale': { tl: 'Benta', en: 'Sale' },
  'stock.reason.Restock': { tl: 'Delivery', en: 'Delivery' },
  'stock.reason.Adjustment': { tl: 'Pagwawasto ng bilang', en: 'Count correction' },
  'stock.reason.Spoilage': { tl: 'Sira', en: 'Spoilage' },
  'stock.reason.Return': { tl: 'Balik', en: 'Return' },
  'stock.reason.RepackOut': { tl: 'Binuksan para tingiin', en: 'Opened for tingi' },
  'stock.reason.RepackIn': { tl: 'Galing sa binuksang pakete', en: 'From an opened pack' }
};
