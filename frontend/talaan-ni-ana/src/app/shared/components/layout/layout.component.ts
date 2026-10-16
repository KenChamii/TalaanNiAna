import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme/theme.service';
import { ToastComponent } from '../toast/toast.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, ToastComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  private auth = inject(AuthService);

  readonly theme = inject(ThemeService);

  /** From the auth response at login. */
  readonly storeName = this.auth.storeName;
  readonly displayName = this.auth.displayName;

  navItems = [
    { path: '/dashboard', label: 'Dashboard' },
    { path: '/checkout', label: 'Benta' },
    { path: '/inventory', label: 'Imbentaryo' },
    { path: '/customers', label: 'Suki' },
    { path: '/transactions', label: 'Transaksyon' },
    { path: '/reports/z', label: 'Z-Report' },
    { path: '/settings', label: 'Settings' },
  ];

  logout() {
    this.auth.logout();
  }
}
