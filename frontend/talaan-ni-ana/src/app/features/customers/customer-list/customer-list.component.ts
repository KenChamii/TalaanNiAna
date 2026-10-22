import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CustomerService } from '../customer.service';
import { CustomerFormComponent } from './customer-form/customer-form.component';
import { SettingsService } from '../../settings/settings.service';
import { Customer } from '../../../shared/models/customer.model';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';
import { ToastService } from '../../../shared/services/toast.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, CustomerFormComponent, PesoPipe, TranslatePipe],
  templateUrl: './customer-list.component.html',
  styleUrl: './customer-list.component.scss'
})
export class CustomerListComponent implements OnInit {
  private customerService = inject(CustomerService);
  private settingsService = inject(SettingsService);
  private toast = inject(ToastService);
  private i18n = inject(I18nService);

  customers = signal<Customer[]>([]);
  searchTerm = signal('');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  isModalOpen = signal(false);

  /** The store's own wording, falling back if settings have not loaded. */
  private reminderTemplate =
    'Hi {name}, paalala lang na may natitirang utang kang {amount} sa tindahan. Salamat po!';

  ngOnInit() {
    this.load();
    this.settingsService.get().subscribe(settings => {
      if (settings.smsAlertTemplate?.trim()) {
        this.reminderTemplate = settings.smsAlertTemplate;
      }
    });
  }

  load() {
    this.customerService.getAll(this.searchTerm(), this.currentPage()).subscribe(result => {
      this.customers.set(result.items);
      this.totalPages.set(result.totalPages);
      this.totalCount.set(result.totalCount);
    });
  }

  onSearchChange() {
    this.currentPage.set(1);
    this.load();
  }

  goToPage(page: number) {
    this.currentPage.set(page);
    this.load();
  }

  copyReminder(customer: Customer) {
    const message = this.reminderTemplate
      .replace('{name}', customer.fullName)
      .replace('{amount}', `₱${customer.totalCredit.toFixed(2)}`);

    navigator.clipboard.writeText(message)
      .then(() => this.toast.success(this.i18n.translate('customers.copied')))
      .catch(() => this.toast.warning(this.i18n.translate('customers.clipboardFailed')));
  }

  onModalSaved(name: string) {
    this.isModalOpen.set(false);
    this.searchTerm.set('');
    this.currentPage.set(1);
    this.load();
    this.toast.success(this.i18n.translate('customers.added', { name }));
  }
}
