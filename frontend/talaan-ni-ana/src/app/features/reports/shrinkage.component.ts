import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService } from '../stock/stock.service';
import { ShrinkageReport } from '../stock/stock.model';
import { PesoPipe } from '../../shared/pipes/peso.pipes';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-shrinkage',
  standalone: true,
  imports: [CommonModule, FormsModule, PesoPipe, TranslatePipe],
  templateUrl: './shrinkage.component.html',
  styleUrl: './shrinkage.component.scss'
})
export class ShrinkageComponent implements OnInit {
  private stock = inject(StockService);

  readonly maxDate = new Date().toISOString().slice(0, 10);

  report = signal<ShrinkageReport | null>(null);
  isLoading = signal(false);

  from = '';
  to = '';

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);

    this.stock.shrinkage(this.from || undefined, this.to || undefined).subscribe({
      next: report => {
        this.report.set(report);
        this.from = report.from;
        this.to = report.to;
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  /** Grouped so the owner sees which items keep going missing. */
  byProduct(report: ShrinkageReport) {
    const totals = new Map<string, { pieces: number; cost: number }>();

    for (const line of report.lines) {
      const running = totals.get(line.productName) ?? { pieces: 0, cost: 0 };
      running.pieces += -line.quantity;
      running.cost += line.costLost;
      totals.set(line.productName, running);
    }

    return [...totals.entries()]
      .map(([name, totalsForName]) => ({ name, ...totalsForName }))
      .sort((a, b) => b.cost - a.cost);
  }
}
