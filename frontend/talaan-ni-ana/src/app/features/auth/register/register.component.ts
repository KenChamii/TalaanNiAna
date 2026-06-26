import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  errorMessage = signal<string | null>(null);
  isLoading = signal(false);

  form = this.fb.group({
    storeName: ['', Validators.required],
    displayName: ['', Validators.required],
    username: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  });

  submit() {
    if (this.form.invalid) return;

    const { username, password, confirmPassword, displayName, storeName } = this.form.value;

    if (password !== confirmPassword) {
      this.errorMessage.set('Hindi tugma ang password.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.auth.register(username!, password!, displayName!, storeName!).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        const apiMessage = err?.error?.message ?? 'Hindi nagawa ang account. Subukan ulit.';
        this.errorMessage.set(apiMessage);
        this.isLoading.set(false);
      }
    });
  }
}