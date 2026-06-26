import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CustomerService } from '../customer.service';
import { CustomerFormComponent } from './customer-form/customer-form.component';
import { Customer } from '../../../shared/models/customer.model';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, CustomerFormComponent, PesoPipe],
  templateUrl: './customer-list.component.html',
  styleUrl: './customer-list.component.scss'
})
export class CustomerListComponent implements OnInit {
  customers = signal<Customer[]>([]);
  searchTerm = signal('');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  isModalOpen = signal(false);

  constructor(private customerService: CustomerService) {}

  ngOnInit() {
    this.load();
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

  copyReminder(customer: Customer) {
    const message = `Hi ${customer.fullName}, paalala lang na may natitirang utang kang ₱${customer.totalCredit.toFixed(2)} sa tindahan. Salamat po!`;
    navigator.clipboard.writeText(message);
    alert('Na-copy sa clipboard! I-paste sa Messenger o SMS.');
  }

  onModalSaved() {
    this.isModalOpen.set(false);
    this.load();
  }
}