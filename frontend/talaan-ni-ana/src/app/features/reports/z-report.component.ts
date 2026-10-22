import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ZReportService } from './z-report.service';
import { ZReport } from './z-report.model';
import { PesoPipe } from '../../shared/pipes/peso.pipes';
import { I18nService } from '../../core/i18n/i18n.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-z-report',
  standalone: true,
  imports: [CommonModule, FormsModule, PesoPipe, TranslatePipe],
  templateUrl: './z-report.component.html',
  styleUrl: './z-report.component.scss'
})
export class ZReportComponent implements OnInit {
  private service = inject(ZReportService);
  private i18n = inject(I18nService);

  report = signal<ZReport | null>(null);
  isLoading = signal(false);

  /** Bound to the date picker. Empty means today in the store's timezone. */
  selectedDate = '';

  /** Future dates are rejected by the API, so the picker stops them first. */
  readonly maxDate = new Date().toISOString().slice(0, 10);

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);

    this.service.get(this.selectedDate || undefined).subscribe({
      next: report => {
        this.report.set(report);
        this.selectedDate = report.businessDate;
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  print() {
    window.print();
  }

  /** Long-form date in the current language, for the printed header. */
  formatDate(value: string): string {
    return new Date(`${value}T00:00:00`).toLocaleDateString(this.i18n.locale(), {
      weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
    });
  }
}
