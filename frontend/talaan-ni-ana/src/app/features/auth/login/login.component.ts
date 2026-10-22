import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { BanderitasComponent } from '../../../shared/components/banderitas/banderitas.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslatePipe, BanderitasComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly i18n = inject(I18nService);

  errorKey = signal<string | null>(null);
  isLoading = signal(false);
  isStartingDemo = signal(false);

  form = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required],
    rememberMe: [true]
  });

  /** One tap into a seeded store, for anyone who does not want to sign up. */
  startDemo() {
    if (this.isStartingDemo()) return;

    this.isStartingDemo.set(true);
    this.errorKey.set(null);

    this.auth.demo().subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => {
        this.errorKey.set('auth.demoFailed');
        this.isStartingDemo.set(false);
      }
    });
  }

  /** Signs in with the entered credentials and opens the dashboard. */
  submit() {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    this.errorKey.set(null);

    const { username, password } = this.form.value;
    this.auth.login(username!, password!).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => {
        this.errorKey.set('auth.badCredentials');
        this.isLoading.set(false);
      }
    });
  }
}
