import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { DecimalPipe, NgFor, NgIf, NgStyle } from '@angular/common';
import { curveMonotoneX } from 'd3-shape';
import { DashboardService } from './dashboard.service';
import { PesoPipe } from '../../shared/pipes/peso.pipes';
import { I18nService } from '../../core/i18n/i18n.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

interface TopProduct {
  productId: number;
  productName: string;
  totalQuantitySold: number;
  totalRevenue: number;
  totalProfit: number;
}

interface TopDebtor { customerId: number; fullName: string; totalCredit: number; }

interface SalesPoint { date: string; cashSales: number; creditSales: number; }

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgxChartsModule, PesoPipe, DecimalPipe, NgFor, NgIf, NgStyle, TranslatePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private i18n = inject(I18nService);

  todaySales = signal(0);
  todayProfit = signal(0);
  collections = signal(0);
  totalCredit = signal(0);
  lowStockCount = signal(0);
  creditorCount = signal(0);
  averageTransactionValue = signal(0);
  monthlyTransactionCount = signal(0);
  monthRevenue = signal(0);
  monthProfit = signal(0);
  monthMarginPercent = signal(0);
  topProducts = signal<TopProduct[]>([]);
  topProfitable = signal<TopProduct[]>([]);
  topDebtors = signal<TopDebtor[]>([]);
  salesHistory = signal<SalesPoint[]>([]);

  today = computed(() => new Date().toLocaleDateString(this.i18n.locale(), {
    weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
  }));

  chartData = computed(() => [
    {
      name: this.i18n.translate('dashboard.series.cash'),
      series: this.salesHistory().map(point => ({ name: point.date, value: point.cashSales }))
    },
    {
      name: this.i18n.translate('dashboard.series.credit'),
      series: this.salesHistory().map(point => ({ name: point.date, value: point.creditSales }))
    }
  ]);

  pieData = computed(() => [
    { name: this.i18n.translate('dashboard.pie.sales'), value: this.todaySales() },
    { name: this.i18n.translate('dashboard.pie.collections'), value: this.collections() },
    { name: this.i18n.translate('dashboard.pie.credit'), value: this.totalCredit() }
  ]);

  curve = curveMonotoneX;

  lineScheme: any = { domain: ['#C8E832', '#D95C5C'] };

  pieScheme: any = { domain: ['#C8E832', '#3CBDB1', '#D95C5C'] };

  /** Share of today's sales that is kita, shown under the profit card. */
  todayMarginPercent(): number {
    const sales = this.todaySales();
    return sales > 0 ? (this.todayProfit() / sales) * 100 : 0;
  }

  ngOnInit() {
    this.dashboardService.getSummary().subscribe(data => {
      this.todaySales.set(data.todaySales);
      this.todayProfit.set(data.todayProfit ?? 0);
      this.collections.set(data.totalCollections);
      this.totalCredit.set(data.totalCredit);
      this.lowStockCount.set(data.lowStockCount);
      this.creditorCount.set(data.creditorCount ?? 0);
      this.averageTransactionValue.set(data.averageTransactionValue ?? 0);
      this.monthlyTransactionCount.set(data.monthlyTransactionCount ?? 0);
      this.monthRevenue.set(data.monthRevenue ?? 0);
      this.monthProfit.set(data.monthProfit ?? 0);
      this.monthMarginPercent.set(data.monthMarginPercent ?? 0);
      this.topProducts.set(data.topProducts ?? []);
      this.topProfitable.set(data.topProfitable ?? []);
      this.topDebtors.set(data.topDebtors ?? []);
      this.salesHistory.set(data.salesHistory ?? []);
    });
  }
}
