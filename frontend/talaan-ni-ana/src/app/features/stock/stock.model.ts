/** Reasons the owner may record by hand. The API rejects anything else. */
export const MANUAL_REASONS = [
  { value: 'Restock', label: 'Delivery', hint: 'Dumating na paninda', sign: 1 },
  { value: 'Return', label: 'Balik', hint: 'Isinauli ng suki', sign: 1 },
  { value: 'Spoilage', label: 'Sira', hint: 'Nasira, naexpire, nabasag', sign: -1 },
  { value: 'Adjustment', label: 'Bilang', hint: 'Hindi tugma ang bilang', sign: 0 }
] as const;

export type ManualReason = (typeof MANUAL_REASONS)[number]['value'];

export interface StockMovement {
  id: number;
  productId: number;
  productName: string;
  /** Signed. Negative left the shelf, positive came in. */
  quantity: number;
  reason: string;
  balanceAfter: number;
  transactionId: number | null;
  note: string | null;
  occurredAt: string;
}

export interface RecordStockInput {
  productId: number;
  quantity: number;
  reason: ManualReason;
  note?: string;
}

export interface RepackResult {
  parentName: string;
  parentRemaining: number;
  tingiName: string;
  tingiProduced: number;
  tingiRemaining: number;
  unitCost: number;
}

export interface ShrinkageLine {
  productName: string;
  quantity: number;
  reason: string;
  costLost: number;
  note: string | null;
  occurredAt: string;
}

export interface ShrinkageReport {
  from: string;
  to: string;
  totalPieces: number;
  totalCost: number;
  lines: ShrinkageLine[];
}

/** Filipino labels for every reason the ledger can hold, including automatic ones. */
export const REASON_LABELS: Record<string, string> = {
  Opening: 'Simulang bilang',
  Sale: 'Benta',
  Restock: 'Delivery',
  Adjustment: 'Pagbabago ng bilang',
  Spoilage: 'Sira',
  Return: 'Balik',
  RepackOut: 'Binuksan para tingiin',
  RepackIn: 'Galing sa pagtingi'
};
