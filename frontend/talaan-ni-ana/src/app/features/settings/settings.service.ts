import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface StoreSettings {
  storeName: string;
  lowStockAlertLimit: number;
  smsAlertTemplate: string;
  timeZoneId: string;
}

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/settings`;

  get() {
    return this.http.get<StoreSettings>(this.url);
  }

  update(payload: StoreSettings) {
    return this.http.put<StoreSettings>(this.url, payload);
  }
}
