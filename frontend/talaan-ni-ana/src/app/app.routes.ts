import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  // Public landing page. Matched on the empty path only, so the guarded shell
  // below still owns every named route beneath it.
  { path: '', pathMatch: 'full', loadComponent: () =>
      import('./features/landing/landing.component').then(m => m.LandingComponent) },

  { path: 'login', loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./shared/components/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: 'dashboard', loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'inventory', loadComponent: () =>
          import('./features/inventory/inventory-list/inventory-list.component').then(m => m.InventoryListComponent) },
      { path: 'customers', loadComponent: () =>
          import('./features/customers/customer-list/customer-list.component').then(m => m.CustomerListComponent) },
      { path: 'customers/:id', loadComponent: () =>
          import('./features/customers/customer-detail/customer-detail.component').then(m => m.CustomerDetailComponent) },
      { path: 'checkout', loadComponent: () =>
          import('./features/checkout/checkout.component').then(m => m.CheckoutComponent) },
      { path: 'transactions', loadComponent: () =>
          import('./features/transactions/transaction-list/transaction-list.component').then(m => m.TransactionListComponent) },
      { path: 'reports/z', loadComponent: () =>
          import('./features/reports/z-report.component').then(m => m.ZReportComponent) },
      { path: 'reports/shrinkage', loadComponent: () =>
          import('./features/reports/shrinkage.component').then(m => m.ShrinkageComponent) },
      { path: 'settings', loadComponent: () =>
          import('./features/settings/settings.component').then(m => m.SettingsComponent) },
    ]
  },
  { path: '**', redirectTo: '' }
];
