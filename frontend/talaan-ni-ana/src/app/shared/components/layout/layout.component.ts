import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ToastComponent } from '../toast/toast.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, ToastComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  storeName = 'Nanay Ana';

  navItems = [
    { path: '/dashboard', label: 'Dashboard', icon: '📊' },
    { path: '/inventory', label: 'Inventory', icon: '📦' },
    { path: '/customers', label: 'Customers', icon: '👥' },
    { path: '/transactions', label: 'Transactions', icon: '🧾' },
    { path: '/checkout', label: 'Live Checkout', icon: '🛒' },
    { path: '/settings', label: 'Settings', icon: '⚙' },
  ];

  constructor(private auth: AuthService) {}

  logout() {
    this.auth.logout();
  }
}