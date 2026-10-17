import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Product } from '../../shared/models/product.model';
import { SILENT_404 } from '../../core/interceptors/silent-404.token';

export interface CheckoutItem {
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface CheckoutPayload {
  paymentType: 'Cash' | 'Credit';
  customerId: number | null;
  items: CheckoutItem[];
  idempotencyKey: string;
}

export interface CheckoutResult {
  transactionId: number;
  totalAmount: number;
  transactionDate: string;
  wasReplay: boolean;
}

@Injectable({ providedIn: 'root' })
export class CheckoutService {
  private http = inject(HttpClient);

  getQuickItems() {
    return this.http.get<Product[]>(`${environment.apiUrl}/products/quick-items`);
  }

  getCustomers() {
    return this.http.get<any>(`${environment.apiUrl}/customers`);
  }

  /**
   * Scanner lookup. An unknown code is an ordinary outcome at the till, so the
   * 404 is kept out of the global error toast and handled by the caller.
   */
  findByBarcode(code: string) {
    return this.http.get<Product>(
      `${environment.apiUrl}/products/barcode/${encodeURIComponent(code)}`,
      { context: new HttpContext().set(SILENT_404, true) });
  }

  processSale(payload: CheckoutPayload) {
    return this.http.post<CheckoutResult>(`${environment.apiUrl}/checkout`, payload);
  }

  /**
   * One key per attempted sale, reused across retries. A double tap or an offline
   * replay then returns the original transaction instead of selling twice.
   */
  newIdempotencyKey(): string {
    return crypto.randomUUID();
  }
}
