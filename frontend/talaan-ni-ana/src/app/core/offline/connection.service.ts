import { Injectable, NgZone, inject, signal } from '@angular/core';

/**
 * Whether the browser thinks it can reach the network.
 *
 * navigator.onLine is a weak signal: it reports whether there is a network
 * interface, not whether anything answers on it. A sari-sari store on a phone
 * hotspot with no load can be "online" and still fail every request. So this is
 * also set to false by the outbox when a request actually fails, and back to
 * true when one succeeds, which is the only evidence that really counts.
 */
@Injectable({ providedIn: 'root' })
export class ConnectionService {
  private zone = inject(NgZone);

  private readonly state = signal(navigator.onLine);

  readonly isOnline = this.state.asReadonly();

  constructor() {
    // Outside Angular's zone by default, so the updates are brought back in.
    window.addEventListener('online', () => this.zone.run(() => this.state.set(true)));
    window.addEventListener('offline', () => this.zone.run(() => this.state.set(false)));
  }

  /** Called by the outbox when a real request succeeds or fails. */
  reportReachable(reachable: boolean) {
    this.state.set(reachable);
  }
}
