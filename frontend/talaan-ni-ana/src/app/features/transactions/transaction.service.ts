import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { TransactionView } from '../../shared/models/transaction.model';
import { PagedResult } from '../../shared/models/product.model';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private url = `${environment.apiUrl}/transactions`;

  constructor(private http: HttpClient) {}

  getAll(filters: { dateFrom?: string; dateTo?: string; paymentType?: string }, page = 1, pageSize = 10) {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (filters.dateFrom) params = params.set('dateFrom', filters.dateFrom);
    if (filters.dateTo) params = params.set('dateTo', filters.dateTo);
    if (filters.paymentType) params = params.set('paymentType', filters.paymentType);

    return this.http.get<PagedResult<TransactionView>>(this.url, { params });
  }
}