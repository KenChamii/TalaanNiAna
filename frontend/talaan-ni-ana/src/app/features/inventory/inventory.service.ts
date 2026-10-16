import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Product, ProductInput, PagedResult } from '../../shared/models/product.model';

export const INVENTORY_PAGE_SIZE = 10;

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/products`;

  getAll(search = '', category = '', page = 1, pageSize = INVENTORY_PAGE_SIZE) {
    const params = new HttpParams()
      .set('search', search)
      .set('category', category)
      .set('page', page)
      .set('pageSize', pageSize);

    return this.http.get<PagedResult<Product>>(this.url, { params });
  }

  /** Categories already in use, for the filter and the form's suggestions. */
  getCategories() {
    return this.http.get<string[]>(`${this.url}/categories`);
  }

  create(product: ProductInput) {
    return this.http.post<Product>(this.url, product);
  }

  update(id: number, product: ProductInput) {
    return this.http.put<Product>(`${this.url}/${id}`, product);
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
