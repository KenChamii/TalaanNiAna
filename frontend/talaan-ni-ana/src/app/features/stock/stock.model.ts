/** Reasons the owner may record by hand. The API rejects anything else. */
export const MANUAL_REASONS = ['Restock', 'Return', 'Spoilage', 'Adjustment'] as const;

export type ManualReason = (typeof MANUAL_REASONS)[number];

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
