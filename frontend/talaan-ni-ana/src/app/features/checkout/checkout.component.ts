import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CheckoutService } from './checkout.service';
import { PesoPipe } from '../../shared/pipes/peso.pipes';
import { ToastService } from '../../shared/services/toast.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { ReceiptModalComponent, ReceiptData } from '../../shared/components/receipt-modal/receipt-modal.component';

interface QuickItem { id: number; name: string; price: number; }
interface CartLine { productId: number; name: string; price: number; qty: number; }

@Component({
    selector: 'app-checkout',
    standalone: true,
    imports: [CommonModule, FormsModule, PesoPipe, TranslatePipe, ReceiptModalComponent],
    templateUrl: './checkout.component.html',
    styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit, AfterViewInit {
    quickItems = signal<QuickItem[]>([]);
    searchTerm = signal('');
    cart = signal<CartLine[]>([]);
    paymentType = signal<'Cash' | 'Credit'>('Cash');
    selectedCustomerId: number | null = null; // plain property, not signal
    isProcessing = signal(false);
    customers = signal<{ id: number; fullName: string }[]>([]);
    currentReceipt = signal<ReceiptData | null>(null);

    /** Bound to the scan box. A USB scanner types into it and presses Enter. */
    scannedCode = '';
    isScanning = signal(false);
    lastScanned = signal('');

    @ViewChild('scanBox') private scanBox?: ElementRef<HTMLInputElement>;

    private toast = inject(ToastService);
    private i18n = inject(I18nService);
    private checkoutService = inject(CheckoutService);

    /** Held across retries so a failed attempt cannot become two sales. */
    private pendingKey: string | null = null;

    subtotal = computed(() =>
        this.cart().reduce((sum, line) => sum + line.price * line.qty, 0)
    );

    /** Pieces, not lines. What the owner counts back to the customer. */
    itemCount = computed(() =>
        this.cart().reduce((sum, line) => sum + line.qty, 0)
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

    ngAfterViewInit() {
        // The till is used with a scanner more often than a mouse, so the scan
        // box holds focus from the moment the screen opens.
        this.focusScanBox();
    }

    /** Called on Enter, which is what a keyboard-wedge scanner sends last. */
    scan() {
        const code = this.scannedCode.trim();
        if (!code || this.isScanning()) return;

        this.isScanning.set(true);

        this.checkoutService.findByBarcode(code).subscribe({
            next: product => {
                this.addToCart({ id: product.id, name: product.name, price: product.price });
                this.lastScanned.set(product.name);
                this.finishScan();
            },
            error: () => {
                this.lastScanned.set('');
                this.toast.warning(this.i18n.translate('pos.noBarcode', { code }));
                this.finishScan();
            }
        });
    }

    addToCart(item: QuickItem) {
        const existing = this.cart().find(l => l.productId === item.id);
        if (existing) {
            this.changeQty(item.id, 1);
        } else {
            this.cart.update(lines => [...lines, { productId: item.id, name: item.name, price: item.price, qty: 1 }]);
        }
    }

    /** Stepper on each line. Dropping to zero takes the line out. */
    changeQty(productId: number, delta: number) {
        this.cart.update(lines => lines
            .map(l => l.productId === productId ? { ...l, qty: l.qty + delta } : l)
            .filter(l => l.qty > 0));
    }

    removeLine(productId: number) {
        this.cart.update(lines => lines.filter(l => l.productId !== productId));
    }

    clearCart() {
        this.cart.set([]);
        this.lastScanned.set('');
        this.focusScanBox();
    }

    checkout() {
        if (this.cart().length === 0 || this.isProcessing()) return;

        if (this.paymentType() === 'Credit' && !this.selectedCustomerId) {
            this.toast.warning(this.i18n.translate('pos.pickCustomerFirst'));
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

        this.checkoutService.processSale(
            {
                paymentType: paymentTypeSnapshot,
                customerId: this.selectedCustomerId,
                items: lineItemsSnapshot.map(l => ({
                    productId: l.productId,
                    quantity: l.qty,
                    unitPrice: l.price
                })),
                idempotencyKey: this.pendingKey
            },
            this.subtotal(),
            this.itemCount()
        ).subscribe({
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
                this.lastScanned.set('');
                this.isProcessing.set(false);

                this.toast.success(this.i18n.translate(
                    result.isPending
                        ? 'offline.queued'
                        : result.wasReplay ? 'pos.replayed' : 'pos.saved'));
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
        this.focusScanBox();
    }

    /** Clears the box and hands focus back, ready for the next item. */
    private finishScan() {
        this.scannedCode = '';
        this.isScanning.set(false);
        this.focusScanBox();
    }

    private focusScanBox() {
        this.scanBox?.nativeElement.focus();
    }
}
