import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService } from '../stock.service';
import { MANUAL_REASONS, ManualReason, REASON_LABELS, StockMovement } from '../stock.model';
import { Product } from '../../../shared/models/product.model';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'app-stock-movements',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-movements.component.html',
  styleUrl: './stock-movements.component.scss'
})
export class StockMovementsComponent implements OnInit {
  private stock = inject(StockService);
  private toast = inject(ToastService);

  @Input({ required: true }) product!: Product;

  /** Raised whenever stock changed, so the list behind can refresh. */
  @Output() recorded = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  readonly reasons = MANUAL_REASONS;
  readonly labels = REASON_LABELS;

  movements = signal<StockMovement[]>([]);
  isLoading = signal(true);
  isSaving = signal(false);

  reason: ManualReason = 'Restock';
  amount: number | null = null;
  note = '';

  /** How many packs to open, when this product is a tingi line. */
  packsToOpen = 1;
  isRepacking = signal(false);

  /** Running balance, updated as movements are recorded. */
  balance = signal(0);

  ngOnInit() {
    this.balance.set(this.product.stockQuantity);
    this.load();
  }

  load() {
    this.isLoading.set(true);

    this.stock.history(this.product.id).subscribe({
      next: list => {
        this.movements.set(list);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  /**
   * A count correction is the only reason the owner states as a total rather
   * than a change, so it is converted to the difference the ledger stores.
   */
  private signedQuantity(): number {
    const amount = this.amount ?? 0;
    if (this.reason === 'Adjustment') return amount - this.balance();
    if (this.reason === 'Spoilage') return -Math.abs(amount);
    return Math.abs(amount);
  }

  get isCount(): boolean {
    return this.reason === 'Adjustment';
  }

  get preview(): number {
    return this.isCount ? (this.amount ?? 0) : this.balance() + this.signedQuantity();
  }

  /** Breaks packs open. Both sides move on the server, in one transaction. */
  repack() {
    if (this.isRepacking() || this.packsToOpen < 1) return;

    this.isRepacking.set(true);

    this.stock.repack(this.product.id, this.packsToOpen).subscribe({
      next: result => {
        this.balance.set(result.tingiRemaining);
        this.isRepacking.set(false);
        this.recorded.emit();
        this.load();
        this.toast.success(
          `Nabuksan ang ${this.packsToOpen} ${result.parentName}. ` +
          `+${result.tingiProduced} ${result.tingiName}.`);
      },
      error: () => this.isRepacking.set(false)
    });
  }

  submit() {
    const quantity = this.signedQuantity();

    if (this.amount === null || quantity === 0) {
      this.toast.warning('Walang pagbabago sa stock.');
      return;
    }

    this.isSaving.set(true);

    this.stock.record({
      productId: this.product.id,
      quantity,
      reason: this.reason,
      note: this.note.trim() || undefined
    }).subscribe({
      next: movement => {
        this.balance.set(movement.balanceAfter);
        this.amount = null;
        this.note = '';
        this.isSaving.set(false);
        this.recorded.emit();
        this.load();
        this.toast.success(`Naitala. ${movement.balanceAfter} ang natitira.`);
      },
      error: () => this.isSaving.set(false)
    });
  }
}
