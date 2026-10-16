import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../inventory.service';
import { InventoryFormComponent } from '../inventory-form/inventory-form.component';
import { Product } from '../../../shared/models/product.model';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'app-inventory-list',
  standalone: true,
  imports: [CommonModule, FormsModule, InventoryFormComponent, PesoPipe],
  templateUrl: './inventory-list.component.html',
  styleUrl: './inventory-list.component.scss'
})
export class InventoryListComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private toast = inject(ToastService);

  products = signal<Product[]>([]);
  categories = signal<string[]>([]);

  searchTerm = signal('');
  selectedCategory = signal('');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);

  isModalOpen = signal(false);
  editingProduct = signal<Product | null>(null);

  ngOnInit() {
    this.load();
    this.loadCategories();
  }

  load() {
    this.inventoryService
      .getAll(this.searchTerm(), this.selectedCategory(), this.currentPage())
      .subscribe(result => {
        this.products.set(result.items);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);
      });
  }

  /** Real categories, so a typed one is filterable instead of invisible. */
  loadCategories() {
    this.inventoryService.getCategories().subscribe(list => this.categories.set(list));
  }

  onSearchChange() {
    this.currentPage.set(1);
    this.load();
  }

  goToPage(page: number) {
    this.currentPage.set(page);
    this.load();
  }

  openAddModal() {
    this.editingProduct.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(product: Product) {
    this.editingProduct.set(product);
    this.isModalOpen.set(true);
  }

  onModalSaved(saved: Product) {
    const wasAdding = this.editingProduct() === null;
    this.isModalOpen.set(false);

    // A new product sorts by name, so it rarely lands on the page you are
    // looking at. Clear the filters and go to it rather than seeming to lose it.
    if (wasAdding) {
      this.searchTerm.set('');
      this.selectedCategory.set('');
      this.currentPage.set(1);
      this.loadCategories();
    }

    this.load();
    this.toast.success(
      wasAdding ? `Nadagdag na ang "${saved.name}".` : `Na-update ang "${saved.name}".`
    );
  }

  deleteProduct(product: Product) {
    if (!confirm(`Tanggalin ang "${product.name}"?`)) return;

    this.inventoryService.delete(product.id).subscribe(() => {
      this.toast.success(`Na-tanggal ang "${product.name}".`);
      this.load();
      this.loadCategories();
    });
  }
}
