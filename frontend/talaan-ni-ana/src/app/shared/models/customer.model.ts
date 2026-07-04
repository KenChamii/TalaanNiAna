export interface Customer {
  id: number;
  fullName: string;
  contactNumber: string;
  totalCredit: number;
  lastActiveAt: string;
}

export interface CustomerTransaction {
  id: number;
  transactionDate: string;
  totalAmount: number;
  paymentType: 'Cash' | 'Credit';
  itemSummaries: string[];
}

export interface CustomerDetail extends Customer {
  transactions: CustomerTransaction[];
}

export interface CreditPaymentResult {
  id: number;
  amount: number;
  paidAt: string;
  note: string | null;
  remainingCredit: number;
}