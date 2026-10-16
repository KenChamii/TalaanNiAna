import { Component, EventEmitter, Input, Output, OnInit, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { InventoryService } from '../inventory.service';
import { Product, ProductInput } from '../../../shared/models/product.model';

@Component({
  selector: 'app-inventory-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './inventory-form.component.html',
  styleUrl: './inventory-form.component.scss'
})
export class InventoryFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private inventoryService = inject(InventoryService);

  @Input() product: Product | null = null;

  /** Existing categories, offered as suggestions without forcing a choice. */
  @Input() categories: string[] = [];

  @Output() saved = new EventEmitter<Product>();
  @Output() closed = new EventEmitter<void>();

  isSaving = false;

  form = this.fb.group({
    name: ['', Validators.required],
    category: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0.01)]],
    // Zero is valid: the owner may not know the puhunan yet.
    costPrice: [0, [Validators.required, Validators.min(0)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
    lowStockThreshold: [10, [Validators.required, Validators.min(0)]]
  });

  private value = toSignal(this.form.valueChanges, { initialValue: this.form.value });

  /** Peso kita per piece, recalculated as the owner types. */
  margin = computed(() => (this.value().price ?? 0) - (this.value().costPrice ?? 0));

  marginPercent = computed(() => {
    const price = this.value().price ?? 0;
    return price > 0 ? (this.margin() / price) * 100 : 0;
  });

  /** Cost above price is allowed, but worth flagging before saving. */
  isLossLeader = computed(() => (this.value().costPrice ?? 0) > 0 && this.margin() < 0);

  ngOnInit() {
    if (this.product) {
      this.form.patchValue({
        name: this.product.name,
        category: this.product.category,
        price: this.product.price,
        costPrice: this.product.costPrice,
        stockQuantity: this.product.stockQuantity,
        lowStockThreshold: this.product.lowStockThreshold
      });
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const raw = this.form.getRawValue();

    const payload: ProductInput = {
      name: raw.name!,
      category: raw.category!.trim(),
      price: raw.price!,
      costPrice: raw.costPrice!,
      stockQuantity: raw.stockQuantity!,
      lowStockThreshold: raw.lowStockThreshold!
    };

    const request = this.product
      ? this.inventoryService.update(this.product.id, payload)
      : this.inventoryService.create(payload);

    // The error interceptor reports the server's reason, so there is nothing
    // useful to add here beyond re-enabling the button.
    request.subscribe({
      next: result => this.saved.emit(result),
      error: () => { this.isSaving = false; }
    });
  }
}
