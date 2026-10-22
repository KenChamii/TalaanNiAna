import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { BanderitasComponent } from '../../../shared/components/banderitas/banderitas.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslatePipe, BanderitasComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly i18n = inject(I18nService);

  readonly steps = ['auth.step1', 'auth.step2', 'auth.step3'];

  errorKey = signal<string | null>(null);
  isLoading = signal(false);

  form = this.fb.group({
    storeName: ['', Validators.required],
    displayName: ['', Validators.required],
    username: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  });

  /** Creates the store and its owner account, then opens the dashboard. */
  submit() {
    if (this.form.invalid) return;

    const { username, password, confirmPassword, displayName, storeName } = this.form.value;

    if (password !== confirmPassword) {
      this.errorKey.set('auth.passwordMismatch');
      return;
    }

    this.isLoading.set(true);
    this.errorKey.set(null);

    this.auth.register(username!, password!, displayName!, storeName!).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: err => {
        const codeKey = `error.code.${err?.error?.code}`;
        this.errorKey.set(this.i18n.has(codeKey) ? codeKey : 'auth.registerFailed');
        this.isLoading.set(false);
      }
    });
  }
}
