import { Component, EventEmitter, Output, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CustomerService } from '../../customer.service';
import { Customer } from '../../../../shared/models/customer.model';
import { ToastService } from '../../../../shared/services/toast.service';

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
  private toast = inject(ToastService);

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
    next: () => {
      this.toast.success('Panibagong suki ang nadagdag!');
      this.saved.emit();
    },
    error: () => {
      this.isSaving = false;
      this.toast.error('Hindi na-dagdag ang suki. Subukan muli.');
    }
  });
}
}
