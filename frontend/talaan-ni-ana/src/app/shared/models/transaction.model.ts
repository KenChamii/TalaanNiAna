export interface TransactionItemView {
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface TransactionView {
  id: number;
  transactionDate: string;
  totalAmount: number;
  paymentType: 'Cash' | 'Credit';
  customerName: string | null;
  items: TransactionItemView[];
}