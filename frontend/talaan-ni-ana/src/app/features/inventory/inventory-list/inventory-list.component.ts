import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../inventory.service';
import { InventoryFormComponent } from '../inventory-form/inventory-form.component';
import { Product } from '../../../shared/models/product.model';
import { PesoPipe } from '../../../shared/pipes/peso.pipes';

@Component({
  selector: 'app-inventory-list',
  standalone: true,
  imports: [CommonModule, FormsModule, InventoryFormComponent, PesoPipe],
  templateUrl: './inventory-list.component.html',
  styleUrl: './inventory-list.component.scss'
})
export class InventoryListComponent implements OnInit {
  products = signal<Product[]>([]);
  searchTerm = signal('');
  selectedCategory = signal('');
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);

  isModalOpen = signal(false);
  editingProduct = signal<Product | null>(null);

  categories = ['Noodles', 'Coffee', 'Beverages', 'Canned Goods', 'Snacks', 'Household'];

  constructor(private inventoryService: InventoryService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.inventoryService
      .getAll(this.searchTerm(), this.selectedCategory(), this.currentPage())
      .subscribe(result => {
        this.products.set(result.items);
        this.totalPages.set(result.totalPages);
        this.totalCount.set(result.totalCount);
      });
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

  onModalSaved() {
    this.isModalOpen.set(false);
    this.load();
  }

  deleteProduct(product: Product) {
    if (!confirm(`Tanggalin ang "${product.name}"?`)) return;
    this.inventoryService.delete(product.id).subscribe(() => this.load());
  }
}