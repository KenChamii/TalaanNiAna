import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PesoPipe } from '../../pipes/peso.pipes';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';

export interface ReceiptLine {
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface ReceiptData {
  transactionId: number;
  transactionDate: string | Date;
  items: ReceiptLine[];
  totalAmount: number;
  paymentType: 'Cash' | 'Credit';
  customerName?: string | null;
}

@Component({
  selector: 'app-receipt-modal',
  standalone: true,
  imports: [CommonModule, PesoPipe, TranslatePipe],
  templateUrl: './receipt-modal.component.html',
  styleUrl: './receipt-modal.component.scss'
})
export class ReceiptModalComponent {
  @Input() receipt: ReceiptData | null = null;
  @Output() closed = new EventEmitter<void>();

  close() {
    this.closed.emit();
  }

  print() {
    window.print();
  }

  onBackdropClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.close();
    }
  }
}
