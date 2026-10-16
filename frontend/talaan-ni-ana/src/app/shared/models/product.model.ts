export interface Product {
  id: number;
  name: string;
  category: string;
  price: number;
  stockQuantity: number;
  lowStockThreshold: number;
  isLowStock: boolean;
}

/** What the form posts. Omitting lowStockThreshold keeps the current value. */
export interface ProductInput {
  name: string;
  category: string;
  price: number;
  stockQuantity: number;
  lowStockThreshold?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
