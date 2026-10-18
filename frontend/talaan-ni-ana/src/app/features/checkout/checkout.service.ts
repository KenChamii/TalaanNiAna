import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Product } from '../../shared/models/product.model';
import { SILENT_404 } from '../../core/interceptors/silent-404.token';
import { ConnectionService } from '../../core/offline/connection.service';
import { OutboxService } from '../../core/offline/outbox.service';
import { Observable, catchError, of, tap, throwError } from 'rxjs';

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
  /** True when the sale is held locally and has not reached the server yet. */
  isPending?: boolean;
}

@Injectable({ providedIn: 'root' })
export class CheckoutService {
  private http = inject(HttpClient);
  private connection = inject(ConnectionService);
  private outbox = inject(OutboxService);

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

  /**
   * Sends the sale, or keeps it if the till cannot reach the server.
   *
   * Queuing returns a result rather than an error, because from the cashier's
   * side the sale happened: the customer paid and walked away. The receipt
   * says it is waiting, and the banner tracks it until it is through.
   */
  processSale(payload: CheckoutPayload, total: number, itemCount: number): Observable<CheckoutResult> {
    if (!this.connection.isOnline()) {
      return this.queue(payload, total, itemCount);
    }

    return this.http.post<CheckoutResult>(`${environment.apiUrl}/checkout`, payload).pipe(
      tap(() => this.connection.reportReachable(true)),
      catchError((error: HttpErrorResponse) => {
        // A rejection is the server speaking, so the cashier must see it. Only
        // an unreachable server turns into a queued sale.
        if (error.status !== 0) return throwError(() => error);

        this.connection.reportReachable(false);
        return this.queue(payload, total, itemCount);
      })
    );
  }

  private queue(payload: CheckoutPayload, total: number, itemCount: number): Observable<CheckoutResult> {
    const ringUpAt = new Date().toISOString();

    void this.outbox.enqueue({ payload, ringUpAt, total, itemCount });

    return of<CheckoutResult>({
      transactionId: 0,            // assigned by the server when it lands
      totalAmount: total,
      transactionDate: ringUpAt,
      wasReplay: false,
      isPending: true
    });
  }

  /**
   * One key per attempted sale, reused across retries. A double tap or an offline
   * replay then returns the original transaction instead of selling twice.
   */
  newIdempotencyKey(): string {
    return crypto.randomUUID();
  }
}
