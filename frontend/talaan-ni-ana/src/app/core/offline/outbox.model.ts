import { CheckoutPayload } from '../../features/checkout/checkout.service';

/**
 * A sale rung up while the till could not reach the server.
 *
 * The payload already carries an idempotency key, generated before the first
 * attempt, so replaying it is safe by construction: the server returns the
 * original transaction rather than selling twice. That key is the reason this
 * queue can exist at all.
 */
export interface PendingSale {
  /** Assigned by IndexedDB. Only used to delete the row once it is through. */
  id?: number;

  payload: CheckoutPayload;

  /** For the receipt and the queue list, since the server's date is not known yet. */
  ringUpAt: string;

  total: number;
  itemCount: number;

  /** How many times sending has been tried, for backing off a poisoned row. */
  attempts: number;

  /** Why the last attempt failed, shown when a sale will not go through. */
  lastError?: string;
}
