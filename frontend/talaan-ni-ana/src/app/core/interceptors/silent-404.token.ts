import { HttpContextToken } from '@angular/common/http';

/**
 * Marks a request whose 404 is a normal result rather than a failure, so the
 * error interceptor stays quiet and the caller handles it. Used by the barcode
 * lookup, where an unknown code is expected at the till.
 */
export const SILENT_404 = new HttpContextToken<boolean>(() => false);
