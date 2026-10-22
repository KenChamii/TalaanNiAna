import { Translations } from '../translation.model';

export const REPORTS: Translations = {
  'reports.loading': { tl: 'Kinukuha ang report…', en: 'Loading the report…' },

  'shrinkage.eyebrow': { tl: 'Nawawalang stock', en: 'Lost stock' },
  'shrinkage.title': { tl: 'Lugi sa sira', en: 'Losses' },
  'shrinkage.piecesLost': { tl: 'Piraso na nawala', en: 'Pieces lost' },
  'shrinkage.costLost': { tl: 'Halaga sa puhunan', en: 'Value at cost' },
  'shrinkage.explainer': {
    tl: 'Puhunan ang ginamit na halaga, hindi presyo. Ito ang talagang nagastos, hindi ang sana ay kinita.',
    en: 'Losses are counted at cost, not selling price: what was actually spent, not what could have been earned.'
  },
  'shrinkage.repeat': { tl: 'Alin ang paulit-ulit', en: 'What keeps going missing' },
  'shrinkage.each': { tl: 'Bawat pangyayari', en: 'Every entry' },
  'shrinkage.empty': { tl: 'Walang naitalang sira o kulang sa panahong ito.', en: 'No spoilage or shortfalls recorded in this period.' },

  'z.eyebrow': { tl: 'Pagsasara ng araw', en: 'End of day' },
  'z.title': { tl: 'Z-Report', en: 'Z-Report' },
  'z.date': { tl: 'Petsa ng report', en: 'Report date' },
  'z.generated': { tl: 'Ginawa noong {time}', en: 'Generated {time}' },

  'z.sales': { tl: 'Benta', en: 'Sales' },
  'z.transactionCount': { tl: 'Bilang ng transaksyon', en: 'Transactions' },
  'z.itemsSold': { tl: 'Piraso na naibenta', en: 'Pieces sold' },
  'z.averageSale': { tl: 'Average kada benta', en: 'Average sale' },
  'z.grossSales': { tl: 'Kabuuang benta', en: 'Gross sales' },

  'z.profit': { tl: 'Kita', en: 'Profit' },
  'z.costOfGoods': { tl: 'Puhunan ng naibenta', en: 'Cost of goods sold' },
  'z.margin': { tl: 'Margin', en: 'Margin' },
  'z.grossProfit': { tl: 'Kitang natira', en: 'Gross profit' },

  'z.drawer': { tl: 'Dapat nasa kaha', en: 'Expected in the drawer' },
  'z.cashSales': { tl: 'Bentang cash', en: 'Cash sales' },
  'z.collections': { tl: 'Bayad sa utang', en: 'Utang payments' },
  'z.expected': { tl: 'Inaasahang laman', en: 'Expected total' },
  'z.drawerNote': { tl: 'Ihambing ito sa aktwal na bilang ng pera bago magsara.', en: 'Compare this with the cash you count before closing.' },

  'z.credit': { tl: 'Utang', en: 'Credit' },
  'z.newCredit': { tl: 'Bagong utang ngayong araw', en: 'New credit today' },
  'z.collected': { tl: 'Nakolekta', en: 'Collected' },
  'z.netChange': { tl: 'Netong galaw', en: 'Net change' },
  'z.outstanding': { tl: 'Kabuuang utang sa ngayon', en: 'Total outstanding' },

  'z.topSellers': { tl: 'Pinakamabenta ngayong araw', en: 'Today’s best sellers' },
  'z.signature': { tl: 'Lagda ng nagsara', en: 'Closed by (signature)' },
  'z.thanks': { tl: 'Salamat po!', en: 'Thank you!' }
};
