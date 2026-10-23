import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/theme/theme.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { BanderitasComponent } from '../../shared/components/banderitas/banderitas.component';
import { SampleDayComponent } from './sample-day/sample-day.component';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe, BanderitasComponent, SampleDayComponent],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly theme = inject(ThemeService);
  readonly i18n = inject(I18nService);
  readonly isLoggedIn = this.auth.isLoggedIn;

  isStartingDemo = signal(false);
  demoFailed = signal(false);

  readonly repoUrl = 'https://github.com/KenChamii/TalaanNiAna';
  readonly authorName = 'Ken Chami';
  readonly authorUrl = 'https://github.com/KenChamii';

  readonly features = [
    'checkout', 'profit', 'credit',
    'closing', 'inventory', 'ledger',
    'tingi', 'offline', 'stores'
  ];

  readonly route = ['till', 'outbox', 'api', 'app', 'db'];

  readonly layers = ['domain', 'application', 'infrastructure', 'api'];

  readonly spec = [
    'backend', 'frontend', 'database', 'auth', 'tenancy',
    'errors', 'offline', 'tests', 'ci', 'hosting', 'languages'
  ];

  readonly limits = [1, 2, 3, 4];

  readonly decisions = ['idempotency', 'timezone', 'pricing', 'errors'];

  /** Signs in to the shared demo store and opens the dashboard. */
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
