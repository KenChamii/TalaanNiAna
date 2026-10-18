import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme/theme.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { ToastComponent } from '../toast/toast.component';
import { OfflineBannerComponent } from '../offline-banner/offline-banner.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule, RouterOutlet, RouterLink, RouterLinkActive,
    ToastComponent, OfflineBannerComponent, TranslatePipe
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly theme = inject(ThemeService);
  readonly i18n = inject(I18nService);

  /** From the auth response at login. */
  readonly storeName = this.auth.storeName;
  readonly displayName = this.auth.displayName;

  /** Off-canvas on a phone, always open from tablet width up. */
  isNavOpen = signal(false);

  navItems = [
    { path: '/dashboard', key: 'nav.dashboard' },
    { path: '/checkout', key: 'nav.checkout' },
    { path: '/inventory', key: 'nav.inventory' },
    { path: '/customers', key: 'nav.customers' },
    { path: '/transactions', key: 'nav.transactions' },
    { path: '/reports/z', key: 'nav.zreport' },
    { path: '/reports/shrinkage', key: 'nav.shrinkage' },
    { path: '/settings', key: 'nav.settings' },
  ];

  constructor() {
    // Tapping a link on a phone should close the menu it was tapped in.
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => this.isNavOpen.set(false));
  }

  toggleNav() {
    this.isNavOpen.update(open => !open);
  }

  logout() {
    this.auth.logout();
  }
}
