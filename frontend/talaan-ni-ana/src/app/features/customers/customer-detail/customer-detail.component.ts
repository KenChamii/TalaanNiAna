import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CustomerService } from '../customer.service';
import { CustomerDetail } from '../../../shared/models/customer.model';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';

@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, PesoPipe],
  templateUrl: './customer-detail.component.html',
  styleUrl: './customer-detail.component.scss'
})
export class CustomerDetailComponent implements OnInit {
  customer = signal<CustomerDetail | null>(null);
  isLoading = signal(true);

  constructor(private route: ActivatedRoute, private customerService: CustomerService) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.customerService.getById(id).subscribe({
      next: data => {
        this.customer.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}