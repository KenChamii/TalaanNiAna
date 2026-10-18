import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/theme/theme.service';

interface Feature {
  tag: string;
  title: string;
  body: string;
  detail: string;
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly theme = inject(ThemeService);
  readonly isLoggedIn = this.auth.isLoggedIn;

  isStartingDemo = signal(false);
  demoFailed = signal(false);

  readonly repoUrl = 'https://github.com/KenChamii/TalaanNiAna';

  /** Banderitas pennants. Colour cycles in CSS. */
  readonly flags = Array.from({ length: 48 });

  readonly features: Feature[] = [
    {
      tag: 'Benta',
      title: 'Till built for one hand',
      body: 'Scan or tap, adjust quantities, take cash or put it on the utang list.',
      detail: 'A USB scanner is a keyboard, so the scan bar holds focus and Enter adds the item. No scanner library, and typing a code works when the scanner is unplugged.'
    },
    {
      tag: 'Puhunan',
      title: 'Cost price and real kita',
      body: 'Every item carries what it cost, so profit is a number rather than a guess.',
      detail: 'Cost is snapshotted onto each sale line, so a supplier raising their price next week never rewrites the profit on sales already made. Loss leaders are allowed and show as negative.'
    },
    {
      tag: 'Utang',
      title: 'The listahan, kept straight',
      body: 'Who owes what, what came in today, and the running total.',
      detail: 'Balances are recomputed from the ledger rather than incremented, so the cached figure cannot drift away from the transactions behind it.'
    },
    {
      tag: 'Pagsasara',
      title: 'End-of-day Z-report',
      body: 'What should be in the drawer at closing, and whether the day made money.',
      detail: 'Cash sales plus utang collections, with credit sales excluded because no money changed hands. Any past day can be reprinted.'
    },
    {
      tag: 'Imbentaryo',
      title: 'Stock that warns you',
      body: 'Per-item thresholds, margin on every line, search by name or barcode.',
      detail: 'Checkout re-reads prices and stock from the database rather than trusting the browser, and an optimistic concurrency token stops two tills selling the same last piece.'
    },
    {
      tag: 'Talaan',
      title: 'Stock with a reason',
      body: 'Delivery, count correction, spoilage, return. Every change is on the record.',
      detail: 'The balance after each movement is stored rather than recomputed, so a correction entered next week cannot rewrite what the owner saw at the time. A shortfall at stock-take is traceable instead of a mystery.'
    },
    {
      tag: 'Tingi',
      title: 'A sachet from a box',
      body: 'Break a pack into pieces and keep both sides of the count honest.',
      detail: 'Repacking moves the pack out and the pieces in within one transaction, and the cost per piece is the pack\'s own cost divided by what it yields, recomputed on every repack because the supplier\'s price moves.'
    },
    {
      tag: 'Walang signal',
      title: 'Keeps selling offline',
      body: 'Sales are held on the phone and sent when the connection comes back.',
      detail: 'Safe to replay because every checkout already carried an idempotency key, so a sale the server recorded comes back as the original rather than being rung up twice. A rejection surfaces at once; only an unreachable server queues.'
    },
    {
      tag: 'Maraming tindahan',
      title: 'One deployment, many stores',
      body: 'Every store sees only its own products, customers and sales.',
      detail: 'Enforced by query filters in the data layer rather than by remembering to add a where clause, so a new query is scoped by default instead of by discipline.'
    }
  ];

  readonly stack = [
    { label: 'Frontend', value: 'Angular 18, standalone components, signals' },
    { label: 'Backend', value: '.NET 10, clean architecture, EF Core 10' },
    { label: 'Database', value: 'PostgreSQL on Neon, Singapore' },
    { label: 'Hosting', value: 'Cloudflare Workers and Render, both free tier' },
    { label: 'Pipeline', value: 'GitHub Actions, build and test on every push' }
  ];

  readonly decisions = [
    {
      title: 'Checkout cannot be made to sell twice',
      body: 'Each attempt carries an idempotency key, unique per store. A double tap, a flaky connection or a retry returns the original sale instead of ringing up another one.'
    },
    {
      title: 'The sales day starts where the store is',
      body: 'Day boundaries are resolved in the store’s own timezone and converted to UTC windows, so a 9pm sale in Manila lands on the day it was made rather than the next UTC one.'
    },
    {
      title: 'Prices are never trusted from the browser',
      body: 'The till sends what it believes the price is; the server compares it to the database and rejects the sale if they disagree, rather than taking the client’s word.'
    },
    {
      title: 'Errors say something useful',
      body: 'Every failure returns an RFC 7807 problem document with a stable code, so the cashier sees "kulang ang stock ng Coke" rather than a generic 500.'
    }
  ];

  readonly roadmap = [
    { state: 'done', label: 'Multi-tenancy, profit tracking, Z-report, barcode scanning' },
    { state: 'done', label: 'PostgreSQL, deployed, continuous delivery' },
    { state: 'done', label: 'Stock movement ledger, so every change has a reason' },
    { state: 'done', label: 'Tingi and repacking, selling a sachet from a box' },
    { state: 'done', label: 'Offline selling, and a Tagalog or English interface' },
    { state: 'next', label: 'Integration tests against a real database, not fakes' },
    { state: 'next', label: 'The rest of the screens translated' },
    { state: 'next', label: 'A nightly reset, so the demo stops drifting' }
  ];

  startDemo() {
    if (this.isStartingDemo()) return;

    this.isStartingDemo.set(true);
    this.demoFailed.set(false);

    this.auth.demo().subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => {
        this.demoFailed.set(true);
        this.isStartingDemo.set(false);
      }
    });
  }
}
