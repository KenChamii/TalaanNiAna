import { Translations } from '../translation.model';

export const CUSTOMERS: Translations = {
  'customers.title': { tl: 'Mga suki at utang', en: 'Customers and credit' },
  'customers.search': { tl: 'Maghanap ng suki…', en: 'Search customers…' },
  'customers.add': { tl: '+ Bagong suki', en: '+ New customer' },
  'customers.col.name': { tl: 'Pangalan', en: 'Name' },
  'customers.col.contact': { tl: 'Contact no.', en: 'Contact no.' },
  'customers.col.credit': { tl: 'Utang', en: 'Balance' },
  'customers.col.lastActive': { tl: 'Huling transaksyon', en: 'Last active' },
  'customers.view': { tl: 'Tingnan', en: 'View' },
  'customers.copyReminder': { tl: 'Kopyahin ang paalala', en: 'Copy reminder' },
  'customers.copied': { tl: 'Nakopya na. I-paste sa Messenger o SMS.', en: 'Copied. Paste it into Messenger or SMS.' },
  'customers.clipboardFailed': { tl: 'Hindi ma-access ang clipboard sa browser na ito.', en: 'This browser doesn’t allow clipboard access.' },
  'customers.added': { tl: 'Nadagdag na si {name}.', en: '{name} has been added.' },

  'customers.form.title': { tl: 'Bagong suki', en: 'New customer' },
  'customers.form.name': { tl: 'Pangalan ng suki', en: 'Customer name' },
  'customers.form.namePlaceholder': { tl: 'hal. Mang Tomas', en: 'e.g. Mang Tomas' },
  'customers.form.contact': { tl: 'Contact no.', en: 'Contact no.' },

  'customers.back': { tl: '← Bumalik sa listahan', en: '← Back to list' },
  'customers.currentCredit': { tl: 'Kasalukuyang utang', en: 'Current balance' },
  'customers.pay': { tl: 'Itala ang bayad sa utang', en: 'Record a payment' },
  'customers.paymentTitle': { tl: 'Itala ang bayad', en: 'Record payment' },
  'customers.amount': { tl: 'Halaga', en: 'Amount' },
  'customers.note': { tl: 'Tala', en: 'Note' },
  'customers.notePlaceholder': { tl: 'hal. bayad noong Lunes', en: 'e.g. paid on Monday' },
  'customers.savePayment': { tl: 'I-save ang bayad', en: 'Save payment' },
  'customers.amountRequired': { tl: 'Maglagay ng halagang mas mataas sa zero.', en: 'Enter an amount greater than zero.' },
  'customers.paidRemaining': { tl: 'Naitala ang bayad. Natitira: {amount}.', en: 'Payment recorded. Remaining balance: {amount}.' },
  'customers.paidFull': { tl: 'Naitala ang bayad. Bayad na ang buong utang.', en: 'Payment recorded. The balance is fully paid.' },
  'customers.history': { tl: 'Kasaysayan ng transaksyon', en: 'Transaction history' },
  'customers.noTransactions': { tl: 'Wala pang transaksyon.', en: 'No transactions yet.' }
};
