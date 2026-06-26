import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CheckoutService } from './checkout.service';
import { PesoPipe } from '../../shared/pipes/peso.pipes';

interface QuickItem { id: number; name: string; price: number; }
interface CartLine { productId: number; name: string; price: number; qty: number; }

@Component({
    selector: 'app-checkout',
    standalone: true,
    imports: [CommonModule, FormsModule, PesoPipe],
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


    subtotal = computed(() =>
        this.cart().reduce((sum, line) => sum + line.price * line.qty, 0)
    );

    filteredItems = computed(() => {
        const term = this.searchTerm().toLowerCase();
        return this.quickItems().filter(i => i.name.toLowerCase().includes(term));
    });

    constructor(private checkoutService: CheckoutService) { }

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
        if (this.cart().length === 0) return;
        if (this.paymentType() === 'Credit' && !this.selectedCustomerId) {
            alert('Pumili ng suki para sa Utang.');
            return;
        }
        this.isProcessing.set(true);
        const payload = {
            paymentType: this.paymentType(),
            customerId: this.selectedCustomerId,
            items: this.cart().map(l => ({ productId: l.productId, quantity: l.qty, unitPrice: l.price }))
        };
        this.checkoutService.processSale(payload).subscribe({
            next: () => { this.cart.set([]); this.selectedCustomerId = null; this.isProcessing.set(false); },
            error: () => this.isProcessing.set(false)
        });
    }
}