import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransactionService } from '../transaction.service';
import { TransactionView } from '../../../shared/models/transaction.model';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PesoPipe],
  templateUrl: './transaction-list.component.html',
  styleUrl: './transaction-list.component.scss'
})
export class TransactionListComponent implements OnInit {
  transactions = signal<TransactionView[]>([]);
  dateFrom = signal('');
  dateTo = signal('');
  paymentType = signal('');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);

  expandedId = signal<number | null>(null);

  constructor(private transactionService: TransactionService) {}
  private toast = inject(ToastService);

  ngOnInit() {
    this.load();
  }

  load() {
    this.transactionService
      .getAll(
        { dateFrom: this.dateFrom(), dateTo: this.dateTo(), paymentType: this.paymentType() },
        this.currentPage()
      )
      .subscribe({
        next: result => {
          this.transactions.set(result.items);
          this.totalPages.set(result.totalPages);
          this.totalCount.set(result.totalCount);
        },
        error: () => this.toast.error('Hindi ma-load ang mga transaksyon.')
      });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.load();
  }

  toggleExpand(id: number) {
    this.expandedId.set(this.expandedId() === id ? null : id);
  }

  goToPage(page: number) {
    this.currentPage.set(page);
    this.load();
  }
}