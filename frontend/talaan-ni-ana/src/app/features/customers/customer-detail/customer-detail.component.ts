import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CustomerService } from '../customer.service';
import { CustomerDetail } from '../../../shared/models/customer.model';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, PesoPipe],
  templateUrl: './customer-detail.component.html',
  styleUrl: './customer-detail.component.scss'
})
export class CustomerDetailComponent implements OnInit {
  customer = signal<CustomerDetail | null>(null);
  isLoading = signal(true);
  private toast = inject(ToastService);

  showPaymentForm = signal(false);
  paymentAmount: number | null = null;
  paymentNote = '';
  isSubmittingPayment = signal(false);

  private customerId!: number;

  constructor(private route: ActivatedRoute, private customerService: CustomerService) {}

  ngOnInit() {
    this.customerId = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
  }

  load() {
    this.customerService.getById(this.customerId).subscribe({
      next: data => {
        this.customer.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.toast.error('Hindi ma-load ang impormasyon ng suki.');
      }
    });
  }

  openPaymentForm() {
    this.paymentAmount = null;
    this.paymentNote = '';
    this.showPaymentForm.set(true);
  }

  cancelPayment() {
    this.showPaymentForm.set(false);
  }

  submitPayment() {
    const current = this.customer();
    if (!current) return;

    if (!this.paymentAmount || this.paymentAmount <= 0) {
      this.toast.warning('Maglagay ng valid na halaga.');
      return;
    }

    if (this.paymentAmount > current.totalCredit) {
      this.toast.warning('Hindi puwedeng lumagpas sa kasalukuyang utang ang bayad.');
      return;
    }

    this.isSubmittingPayment.set(true);
    this.customerService.recordPayment(this.customerId, this.paymentAmount, this.paymentNote || undefined).subscribe({
      next: () => {
        this.isSubmittingPayment.set(false);
        this.showPaymentForm.set(false);
        this.toast.success('Naitala ang bayad!');
        this.load();
      },
      error: () => {
        this.isSubmittingPayment.set(false);
        this.toast.error('Hindi naitala ang bayad. Subukan muli.');
      }
    });
  }
}