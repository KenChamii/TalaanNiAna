import { Component, OnInit, signal, computed } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { NgFor, NgStyle } from '@angular/common';
import { curveMonotoneX } from 'd3-shape';
import { DashboardService } from './dashboard.service';
import { PesoPipe } from '../../shared/pipes/peso.pipes';

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
  chartData = signal<any[]>([]);
  pieData = signal<any[]>([]);

  today = new Date().toLocaleDateString('fil-PH', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });

  collectionRate = computed(() => {
    const sales = this.todaySales();
    return sales > 0 ? Math.round((this.collections() / sales) * 100) : 0;
  });

  curve = curveMonotoneX;

  lineScheme: any = { domain: ['#3B6FE8', '#E5534B'] };

  pieScheme: any = { domain: ['#3B6FE8', '#2ECC71', '#E5534B'] };

  getPieColor(name: string): string {
    const map: Record<string, string> = {
      'Benta': '#3B6FE8',
      'Koleksyon': '#2ECC71',
      'Utang': '#E5534B'
    };
    return map[name] ?? '#6E7A94';
  }

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.dashboardService.getSummary().subscribe(data => {
      this.todaySales.set(data.todaySales);
      this.collections.set(data.totalCollections);
      this.totalCredit.set(data.totalCredit);
      this.lowStockCount.set(data.lowStockCount);
      this.creditorCount.set(data.creditorCount ?? 0);

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