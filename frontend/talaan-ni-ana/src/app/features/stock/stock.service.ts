import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { RecordStockInput, RepackResult, ShrinkageReport, StockMovement } from './stock.model';

@Injectable({ providedIn: 'root' })
export class StockService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/stock`;

  history(productId: number, count = 50) {
    const params = new HttpParams().set('count', count);
    return this.http.get<StockMovement[]>(`${this.url}/movements/${productId}`, { params });
  }

  record(input: RecordStockInput) {
    return this.http.post<StockMovement>(`${this.url}/movements`, input);
  }

  /** Opens packs into tingi. Names the tingi product, not the pack. */
  repack(tingiProductId: number, parentQuantity = 1) {
    return this.http.post<RepackResult>(`${this.url}/repack`, { tingiProductId, parentQuantity });
  }

  /** Omitting the dates gives the last 30 days. */
  shrinkage(from?: string, to?: string) {
    let params = new HttpParams();
    if (from) params = params.set('from', from);
    if (to) params = params.set('to', to);

    return this.http.get<ShrinkageReport>(`${this.url}/shrinkage`, { params });
  }
}
