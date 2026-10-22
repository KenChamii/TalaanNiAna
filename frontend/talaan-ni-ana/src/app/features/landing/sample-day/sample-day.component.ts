import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';

interface SampleLine {
  name: string;
  quantity: number;
  price: number;
  cost: number;
}

@Component({
  selector: 'app-sample-day',
  standalone: true,
  imports: [CommonModule, TranslatePipe, PesoPipe],
  templateUrl: './sample-day.component.html',
  styleUrl: './sample-day.component.scss'
})
export class SampleDayComponent {
  readonly lines: SampleLine[] = [
    { name: 'Softdrinks 290ml', quantity: 12, price: 20, cost: 16.5 },
    { name: 'Pancit canton', quantity: 8, price: 16, cost: 13 },
    { name: 'Kape 3-in-1', quantity: 15, price: 8, cost: 6.5 },
    { name: 'Itlog', quantity: 10, price: 9, cost: 7.5 }
  ];

  readonly sales = this.sum(line => line.price);
  readonly cost = this.sum(line => line.cost);
  readonly profit = this.sales - this.cost;

  /** Totals a per-piece amount across every line. */
  private sum(perPiece: (line: SampleLine) => number): number {
    return this.lines.reduce((total, line) => total + perPiece(line) * line.quantity, 0);
  }
}
