export interface Product {
  id: number;
  name: string;
  category: string;
  /** Null for loose items with nothing printed to scan. */
  barcode: string | null;
  price: number;
  /** Puhunan. Zero means it has not been recorded yet. */
  costPrice: number;
  unitMargin: number;
  marginPercent: number;
  stockQuantity: number;
  lowStockThreshold: number;
  isLowStock: boolean;
  /** The pack this is broken out of, when it is a tingi line. */
  parentProductId: number | null;
  parentProductName: string | null;
  /** How many come out of one pack. */
  unitsPerParent: number | null;
}

/** What the form posts. Omitting a field keeps the current value. */
export interface ProductInput {
  name: string;
  category: string;
  /** Empty string clears the code; omitting it keeps the current one. */
  barcode?: string;
  price: number;
  costPrice?: number;
  stockQuantity: number;
  lowStockThreshold?: number;
  /** Zero clears the tingi link; omitting it keeps the current one. */
  parentProductId?: number;
  unitsPerParent?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
