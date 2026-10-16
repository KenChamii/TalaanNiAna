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
  private route = inject(ActivatedRoute);
  private customerService = inject(CustomerService);
  private toast = inject(ToastService);

  customer = signal<CustomerDetail | null>(null);
  isLoading = signal(true);

  showPaymentForm = signal(false);
  paymentAmount: number | null = null;
  paymentNote = '';
  isSubmittingPayment = signal(false);

  private customerId!: number;

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
      error: () => this.isLoading.set(false)
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
    if (!this.customer()) return;

    // Only the checks the server cannot make cheaply. Overpayment is the
    // server's call, so its message is the one the user should see.
    if (!this.paymentAmount || this.paymentAmount <= 0) {
      this.toast.warning('Maglagay ng halaga na mas mataas sa zero.');
      return;
    }

    this.isSubmittingPayment.set(true);

    this.customerService
      .recordPayment(this.customerId, this.paymentAmount, this.paymentNote || undefined)
      .subscribe({
        next: result => {
          this.isSubmittingPayment.set(false);
          this.showPaymentForm.set(false);
          this.load();
          this.toast.success(
            result.remainingCredit > 0
              ? `Naitala ang bayad. Natitira: ₱${result.remainingCredit.toFixed(2)}.`
              : 'Naitala ang bayad. Wala nang utang.'
          );
        },
        error: () => this.isSubmittingPayment.set(false)
      });
  }
}
