import { Component, OnInit, signal } from '@angular/core';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { DashboardService } from './dashboard.service';
import { PesoPipe } from '../../shared/pipes/peso.pipes';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgxChartsModule, PesoPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  todaySales = signal(0);
  collections = signal(0);
  totalCredit = signal(0);
  lowStockCount = signal(0);
  chartData = signal<any[]>([]);

  colorScheme: any = {
    domain: ['#0038A8', '#CE1126']
  };

  constructor(private dashboardService: DashboardService) {}

  ngOnInit() {
    this.dashboardService.getSummary().subscribe(data => {
      this.todaySales.set(data.todaySales);
      this.collections.set(data.totalCollections);
      this.totalCredit.set(data.totalCredit);
      this.lowStockCount.set(data.lowStockCount);

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
    });
  }
}