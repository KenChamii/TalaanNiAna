import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ZReport } from './z-report.model';

@Injectable({ providedIn: 'root' })
export class ZReportService {
  private http = inject(HttpClient);

  /** Omitting the date gives today in the store's own timezone. */
  get(date?: string) {
    const params = date ? new HttpParams().set('date', date) : undefined;
    return this.http.get<ZReport>(`${environment.apiUrl}/reports/z`, { params });
  }
}
