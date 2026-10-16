import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, catchError, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

/** Returned by login, register and refresh. The refresh token itself is an httpOnly cookie. */
export interface AuthResult {
  accessToken: string;
  expiresInSeconds: number;
  displayName: string;
  storeName: string;
}

const STORE_KEY = 'talaan_store';
const NAME_KEY = 'talaan_display_name';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  /** Kept in memory only, so an XSS payload cannot read it from storage. */
  private accessToken = signal<string | null>(null);

  readonly isLoggedIn = computed(() => this.accessToken() !== null);

  /** Display-only, so these are safe to persist and avoid a blank topbar on reload. */
  readonly storeName = signal(this.read(STORE_KEY) ?? '');
  readonly displayName = signal(this.read(NAME_KEY) ?? '');

  login(username: string, password: string): Observable<AuthResult> {
    return this.post('login', { username, password });
  }

  register(username: string, password: string, displayName: string, storeName: string): Observable<AuthResult> {
    return this.post('register', { username, password, displayName, storeName });
  }

  /** Exchanges the refresh cookie for a new access token. */
  refresh(): Observable<AuthResult | null> {
    return this.http
      .post<AuthResult>(`${environment.apiUrl}/auth/refresh`, {}, { withCredentials: true })
      .pipe(
        tap(result => this.accept(result)),
        catchError(() => {
          this.clear();
          return of(null);
        })
      );
  }

  /** Called once at startup to restore a session from the cookie. */
  restoreSession(): Observable<AuthResult | null> {
    return this.refresh();
  }

  /** Keeps the topbar current when the store is renamed in Settings. */
  setStoreName(name: string): void {
    this.storeName.set(name);
    this.write(STORE_KEY, name);
  }

  logout(): void {
    this.http
      .post(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true })
      .subscribe({
        next: () => this.finishLogout(),
        error: () => this.finishLogout()
      });
  }

  getToken(): string | null {
    return this.accessToken();
  }

  private post(path: 'login' | 'register', body: unknown): Observable<AuthResult> {
    return this.http
      .post<AuthResult>(`${environment.apiUrl}/auth/${path}`, body, { withCredentials: true })
      .pipe(tap(result => this.accept(result)));
  }

  private accept(result: AuthResult): void {
    this.accessToken.set(result.accessToken);
    this.storeName.set(result.storeName ?? '');
    this.displayName.set(result.displayName ?? '');
    this.write(STORE_KEY, result.storeName ?? '');
    this.write(NAME_KEY, result.displayName ?? '');
  }

  private clear(): void {
    this.accessToken.set(null);
    this.storeName.set('');
    this.displayName.set('');
    this.write(STORE_KEY, null);
    this.write(NAME_KEY, null);
  }

  private finishLogout(): void {
    this.clear();
    this.router.navigate(['/login']);
  }

  private read(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch {
      return null;
    }
  }

  private write(key: string, value: string | null): void {
    try {
      if (value === null) localStorage.removeItem(key);
      else localStorage.setItem(key, value);
    } catch {
      // Blocked storage: session still works in memory.
    }
  }
}
