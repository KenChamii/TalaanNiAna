import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private url = `${environment.apiUrl}/settings`;

  constructor(private http: HttpClient) {}

  get() {
    return this.http.get<any>(this.url);
  }

  update(payload: any) {
    return this.http.put(this.url, payload);
  }
}