export interface Product {
  id: number;
  name: string;
  category: string;
  price: number;
  stockQuantity: number;
  isLowStock: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}