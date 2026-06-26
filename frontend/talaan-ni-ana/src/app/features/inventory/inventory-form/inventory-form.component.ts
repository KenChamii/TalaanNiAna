import { Component, EventEmitter, Input, Output, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { InventoryService } from '../inventory.service';
import { Product } from '../../../shared/models/product.model';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'app-inventory-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './inventory-form.component.html',
  styleUrl: './inventory-form.component.scss'
})
export class InventoryFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private inventoryService = inject(InventoryService);
  private toast = inject(ToastService);

  @Input() product: Product | null = null;
  @Output() saved = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  isSaving = false;

  form = this.fb.group({
    name: ['', Validators.required],
    category: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit() {
    if (this.product) {
      this.form.patchValue({
        name: this.product.name,
        category: this.product.category,
        price: this.product.price,
        stockQuantity: this.product.stockQuantity
      });
    }
  }

submit() {
  if (this.form.invalid) return;
  this.isSaving = true;

  const raw = this.form.value;
  const payload: Partial<Product> = {
    name: raw.name ?? undefined,
    category: raw.category ?? undefined,
    price: raw.price ?? undefined,
    stockQuantity: raw.stockQuantity ?? undefined,
  };

  const request = this.product
    ? this.inventoryService.update(this.product.id, payload)
    : this.inventoryService.create(payload);

  request.subscribe({
    next: () => {
      this.toast.success(this.product ? `"${this.product.name}" na-update!` : 'Nadagdag na ang produkto!');
      this.saved.emit();
    },
    error: () => {
      this.isSaving = false;
      this.toast.error('Hindi na-save ang produkto. Subukan muli.');
    }
  });
}
}
