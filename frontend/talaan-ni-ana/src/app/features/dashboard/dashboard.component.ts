import { Component, OnInit, signal } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { NgFor, NgStyle } from '@angular/common';
import { curveMonotoneX } from 'd3-shape';
import { DashboardService } from './dashboard.service';
import { PesoPipe } from '../../shared/pipes/peso.pipes';

interface TopProduct { productId: number; productName: string; totalQuantitySold: number; totalRevenue: number; }
interface TopDebtor { customerId: number; fullName: string; totalCredit: number; }

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgxChartsModule, PesoPipe, NgFor, NgStyle],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  todaySales = signal(0);
  collections = signal(0);
  totalCredit = signal(0);
  lowStockCount = signal(0);
  creditorCount = signal(0);
  averageTransactionValue = signal(0);
  monthlyTransactionCount = signal(0);
  topProducts = signal<TopProduct[]>([]);
  topDebtors = signal<TopDebtor[]>([]);
  chartData = signal<any[]>([]);
  pieData = signal<any[]>([]);

  today = new Date().toLocaleDateString('fil-PH', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });

  curve = curveMonotoneX;

  lineScheme: any = { domain: ['#C8E832', '#D95C5C'] };

  pieScheme: any = { domain: ['#C8E832', '#3CBDB1', '#D95C5C'] };

  getPieColor(name: string): string {
    const map: Record<string, string> = {
      'Benta': '#C8E832',
      'Koleksyon': '#3CBDB1',
      'Utang': '#D95C5C'
    };
    return map[name] ?? '#5C5649';
  }

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.dashboardService.getSummary().subscribe(data => {
      this.todaySales.set(data.todaySales);
      this.collections.set(data.totalCollections);
      this.totalCredit.set(data.totalCredit);
      this.lowStockCount.set(data.lowStockCount);
      this.creditorCount.set(data.creditorCount ?? 0);
      this.averageTransactionValue.set(data.averageTransactionValue ?? 0);
      this.monthlyTransactionCount.set(data.monthlyTransactionCount ?? 0);
      this.topProducts.set(data.topProducts ?? []);
      this.topDebtors.set(data.topDebtors ?? []);

      this.chartData.set([
        {
          name: 'Cash Sales',
          series: data.salesHistory.map((p: any) => ({ name: p.date, value: p.cashSales }))
        },
        {
          name: 'Utang (Credit)',
          series: data.salesHistory.map((p: any) => ({ name: p.date, value: p.creditSales }))
        }
      ]);

      this.pieData.set([
        { name: 'Benta', value: data.todaySales },
        { name: 'Koleksyon', value: data.totalCollections },
        { name: 'Utang', value: data.totalCredit },
      ]);
    });
  }
}