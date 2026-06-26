import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Customer, CustomerDetail } from '../../shared/models/customer.model';
import { PagedResult } from '../../shared/models/product.model';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private url = `${environment.apiUrl}/customers`;

  constructor(private http: HttpClient) {}

  getAll(search = '', page = 1, pageSize = 10) {
    const params = new HttpParams().set('search', search).set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<Customer>>(this.url, { params });
  }

  create(customer: Partial<Customer>) {
    return this.http.post<Customer>(this.url, customer);
  }

  getById(id: number) {
    return this.http.get<CustomerDetail>(`${this.url}/${id}`);
  }
}