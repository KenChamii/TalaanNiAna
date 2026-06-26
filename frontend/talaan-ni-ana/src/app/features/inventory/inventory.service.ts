import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Product, PagedResult } from '../../shared/models/product.model';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private url = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) {}

  getAll(search = '', category = '', page = 1, pageSize = 10) {
    const params = new HttpParams()
      .set('search', search)
      .set('category', category)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PagedResult<Product>>(this.url, { params });
  }

  create(product: Partial<Product>) {
    return this.http.post<Product>(this.url, product);
  }

  update(id: number, product: Partial<Product>) {
    return this.http.put<Product>(`${this.url}/${id}`, product);
  }

  delete(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }
}