import { Component, EventEmitter, Output, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CustomerService } from '../../customer.service';

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

  @Output() saved = new EventEmitter<string>();
  @Output() closed = new EventEmitter<void>();

  isSaving = false;

  form = this.fb.group({
    fullName: ['', Validators.required],
    // Optional on the API too; plenty of suki have no number on file.
    contactNumber: ['']
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const raw = this.form.getRawValue();

    this.customerService.create({
      fullName: raw.fullName!.trim(),
      contactNumber: raw.contactNumber?.trim() || undefined
    }).subscribe({
      next: created => this.saved.emit(created.fullName),
      error: () => { this.isSaving = false; }
    });
  }
}
