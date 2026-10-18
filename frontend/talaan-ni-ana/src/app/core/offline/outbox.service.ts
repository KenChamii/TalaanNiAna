import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ConnectionService } from './connection.service';
import { PendingSale } from './outbox.model';

const DB_NAME = 'talaan-outbox';
const DB_VERSION = 1;
const STORE = 'sales';

/** After this many failures a sale stops being retried automatically. */
const MAX_ATTEMPTS = 5;

/**
 * Sales rung up with no connection, held until one comes back.
 *
 * IndexedDB rather than localStorage, because a sale must survive the tab being
 * closed and a phone running out of battery mid-transaction, and because
 * localStorage is synchronous and would block the till on every write.
 */
@Injectable({ providedIn: 'root' })
export class OutboxService {
  private http = inject(HttpClient);
  private connection = inject(ConnectionService);

  private database?: Promise<IDBDatabase>;
  private flushing = false;

  readonly pending = signal<PendingSale[]>([]);
  readonly count = computed(() => this.pending().length);

  /** Sales that have failed too often to keep retrying on their own. */
  readonly stuck = computed(() => this.pending().filter(s => s.attempts >= MAX_ATTEMPTS));

  constructor() {
    void this.refresh();

    // A reconnection is the only interesting moment: send whatever is waiting.
    window.addEventListener('online', () => void this.flush());
  }

  async enqueue(sale: Omit<PendingSale, 'id' | 'attempts'>): Promise<void> {
    await this.write(store => store.add({ ...sale, attempts: 0 }));
    await this.refresh();
  }

  /**
   * Sends everything waiting, oldest first. Order matters: two sales of the
   * last three pieces must fail in the order they were rung up, not whichever
   * the browser happens to send first.
   */
  async flush(): Promise<void> {
    if (this.flushing) return;

    this.flushing = true;

    try {
      for (const sale of [...this.pending()].sort((a, b) => a.ringUpAt.localeCompare(b.ringUpAt))) {
        if (sale.attempts >= MAX_ATTEMPTS) continue;

        try {
          await firstValueFrom(
            this.http.post(`${environment.apiUrl}/checkout`, sale.payload));

          await this.remove(sale.id!);
          this.connection.reportReachable(true);
        } catch (error) {
          const failure = error as HttpErrorResponse;

          // A rejection is final: the server heard it and said no, so retrying
          // changes nothing. A network error is worth trying again later.
          const serverAnswered = failure.status >= 400 && failure.status < 500;

          await this.write(store => store.put({
            ...sale,
            attempts: serverAnswered ? MAX_ATTEMPTS : sale.attempts + 1,
            lastError: failure.error?.detail ?? failure.message
          }));

          if (!serverAnswered) {
            this.connection.reportReachable(false);
            break;   // nothing else will get through either
          }
        }
      }
    } finally {
      this.flushing = false;
      await this.refresh();
    }
  }

  /** Drops a sale that will never go through, once the owner has seen why. */
  async discard(id: number): Promise<void> {
    await this.remove(id);
    await this.refresh();
  }

  private async refresh(): Promise<void> {
    const database = await this.open();

    const rows = await new Promise<PendingSale[]>((resolve, reject) => {
      const request = database.transaction(STORE, 'readonly').objectStore(STORE).getAll();
      request.onsuccess = () => resolve(request.result as PendingSale[]);
      request.onerror = () => reject(request.error);
    });

    this.pending.set(rows);
  }

  private async remove(id: number): Promise<void> {
    await this.write(store => store.delete(id));
  }

  private async write(action: (store: IDBObjectStore) => IDBRequest): Promise<void> {
    const database = await this.open();

    await new Promise<void>((resolve, reject) => {
      const transaction = database.transaction(STORE, 'readwrite');
      action(transaction.objectStore(STORE));
      transaction.oncomplete = () => resolve();
      transaction.onerror = () => reject(transaction.error);
    });
  }

  private open(): Promise<IDBDatabase> {
    this.database ??= new Promise<IDBDatabase>((resolve, reject) => {
      const request = indexedDB.open(DB_NAME, DB_VERSION);

      request.onupgradeneeded = () => {
        if (!request.result.objectStoreNames.contains(STORE))
          request.result.createObjectStore(STORE, { keyPath: 'id', autoIncrement: true });
      };

      request.onsuccess = () => resolve(request.result);
      request.onerror = () => reject(request.error);
    });

    return this.database;
  }
}
