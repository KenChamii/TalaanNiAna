import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

/** /auth/login and /auth/register response. */
interface AuthResponse {
  token: string;
  displayName: string;
  storeName: string;
}

const TOKEN_KEY = 'talaan_token';
const STORE_KEY = 'talaan_store';
const NAME_KEY = 'talaan_display_name';

@Injectable({ providedIn: 'root' })
export class AuthService {
  isLoggedIn = signal(!!this.read(TOKEN_KEY));

  /** Not in the JWT, so kept here from the auth response. */
  storeName = signal(this.read(STORE_KEY) ?? '');
  displayName = signal(this.read(NAME_KEY) ?? '');

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, { username, password })
      .pipe(tap(res => this.accept(res)));
  }

  register(username: string, password: string, displayName: string, storeName: string) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, {
        username,
        password,
        displayName,
        storeName
      })
      .pipe(tap(res => this.accept(res)));
  }

  logout() {
    this.write(TOKEN_KEY, null);
    this.write(STORE_KEY, null);
    this.write(NAME_KEY, null);
    this.isLoggedIn.set(false);
    this.storeName.set('');
    this.displayName.set('');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.read(TOKEN_KEY);
  }

  private accept(res: AuthResponse) {
    this.write(TOKEN_KEY, res.token);
    this.write(STORE_KEY, res.storeName ?? '');
    this.write(NAME_KEY, res.displayName ?? '');
    this.isLoggedIn.set(true);
    this.storeName.set(res.storeName ?? '');
    this.displayName.set(res.displayName ?? '');
  }

  private read(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch {
      return null;
    }
  }

  private write(key: string, value: string | null) {
    try {
      if (value === null) localStorage.removeItem(key);
      else localStorage.setItem(key, value);
    } catch {
      // Blocked storage: session still works in memory.
    }
  }
}
