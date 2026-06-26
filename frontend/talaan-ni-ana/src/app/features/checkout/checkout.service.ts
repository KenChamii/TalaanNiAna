import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CheckoutService {
  constructor(private http: HttpClient) {}

  getQuickItems() {
    return this.http.get<any[]>(`${environment.apiUrl}/products/quick-items`);
  }

  processSale(payload: any) {
    return this.http.post(`${environment.apiUrl}/checkout`, payload);
  }

  getCustomers() {
  return this.http.get<any>(`${environment.apiUrl}/customers`);
}
}