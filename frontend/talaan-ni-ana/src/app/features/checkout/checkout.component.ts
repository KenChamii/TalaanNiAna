import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CheckoutService } from './checkout.service';
import { PesoPipe } from '../../shared/pipes/peso.pipes';
import { ToastService } from '../../shared/services/toast.service';
import { ReceiptModalComponent, ReceiptData } from '../../shared/components/receipt-modal/receipt-modal.component';

interface QuickItem { id: number; name: string; price: number; }
interface CartLine { productId: number; name: string; price: number; qty: number; }

@Component({
    selector: 'app-checkout',
    standalone: true,
    imports: [CommonModule, FormsModule, PesoPipe, ReceiptModalComponent],
    templateUrl: './checkout.component.html',
    styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit {
    quickItems = signal<QuickItem[]>([]);
    searchTerm = signal('');
    cart = signal<CartLine[]>([]);
    paymentType = signal<'Cash' | 'Credit'>('Cash');
    selectedCustomerId: number | null = null; // plain property, not signal
    isProcessing = signal(false);
    customers = signal<{ id: number; fullName: string }[]>([]);
    currentReceipt = signal<ReceiptData | null>(null);

    private toast = inject(ToastService);
    private checkoutService = inject(CheckoutService);

    /** Held across retries so a failed attempt cannot become two sales. */
    private pendingKey: string | null = null;

    subtotal = computed(() =>
        this.cart().reduce((sum, line) => sum + line.price * line.qty, 0)
    );

    filteredItems = computed(() => {
        const term = this.searchTerm().toLowerCase();
        return this.quickItems().filter(i => i.name.toLowerCase().includes(term));
    });

    ngOnInit() {
        this.checkoutService.getQuickItems().subscribe(items => this.quickItems.set(items));
        // customers endpoint is paginated — unwrap .items
        this.checkoutService.getCustomers().subscribe(data => this.customers.set(data.items));
    }

    addToCart(item: QuickItem) {
        const existing = this.cart().find(l => l.productId === item.id);
        if (existing) {
            this.cart.update(lines =>
                lines.map(l => l.productId === item.id ? { ...l, qty: l.qty + 1 } : l)
            );
        } else {
            this.cart.update(lines => [...lines, { productId: item.id, name: item.name, price: item.price, qty: 1 }]);
        }
    }

    removeLine(productId: number) {
        this.cart.update(lines => lines.filter(l => l.productId !== productId));
    }

    checkout() {
        if (this.cart().length === 0 || this.isProcessing()) return;

        if (this.paymentType() === 'Credit' && !this.selectedCustomerId) {
            this.toast.warning('Pumili ng suki para sa Utang.');
            return;
        }

        this.isProcessing.set(true);
        this.pendingKey ??= this.checkoutService.newIdempotencyKey();

        // Snapshot the cart and payment state now, since success resets them.
        const lineItemsSnapshot = this.cart();
        const paymentTypeSnapshot = this.paymentType();
        const customerNameSnapshot = paymentTypeSnapshot === 'Credit'
            ? this.customers().find(c => c.id === this.selectedCustomerId)?.fullName ?? null
            : null;

        this.checkoutService.processSale({
            paymentType: paymentTypeSnapshot,
            customerId: this.selectedCustomerId,
            items: lineItemsSnapshot.map(l => ({
                productId: l.productId,
                quantity: l.qty,
                unitPrice: l.price
            })),
            idempotencyKey: this.pendingKey
        }).subscribe({
            next: result => {
                this.currentReceipt.set({
                    transactionId: result.transactionId,
                    transactionDate: result.transactionDate,
                    items: lineItemsSnapshot.map(l => ({
                        productName: l.name,
                        quantity: l.qty,
                        unitPrice: l.price,
                        subtotal: l.price * l.qty
                    })),
                    totalAmount: result.totalAmount,
                    paymentType: paymentTypeSnapshot,
                    customerName: customerNameSnapshot
                });

                this.cart.set([]);
                this.selectedCustomerId = null;
                this.pendingKey = null;
                this.isProcessing.set(false);

                this.toast.success(result.wasReplay ? 'Naitala na ang bentang ito.' : 'Nasave na ang benta!');
            },
            error: () => {
                // The key is kept so a retry replays rather than duplicating.
                // The error interceptor already showed the server's message.
                this.isProcessing.set(false);
            }
        });
    }

    closeReceipt() {
        this.currentReceipt.set(null);
    }
}
