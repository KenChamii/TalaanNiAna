import { Component, EventEmitter, Output, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CustomerService } from '../../customer.service';
import { Customer } from '../../../../shared/models/customer.model';

@Component({
  selector: 'app-customer-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './customer-form.component.html',
  styleUrl: './customer-form.component.scss'
})
export class CustomerFormComponent {
  private fb = inject(FormBuilder);
  private customerService = inject(CustomerService);

  @Output() saved = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();
  isSaving = false;

  form = this.fb.group({
    fullName: ['', Validators.required],
    contactNumber: ['', Validators.required]
  });

 submit() {
  if (this.form.invalid) return;
  this.isSaving = true;

  const raw = this.form.value;
  const payload: Partial<Customer> = {
    fullName: raw.fullName ?? undefined,
    contactNumber: raw.contactNumber ?? undefined,
  };

  this.customerService.create(payload).subscribe({
    next: () => this.saved.emit(),
    error: () => (this.isSaving = false)
  });
}
}