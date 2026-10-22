import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { SettingsService, StoreSettings } from './settings.service';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../shared/services/toast.service';
import { I18nService } from '../../core/i18n/i18n.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private settingsService = inject(SettingsService);
  private auth = inject(AuthService);
  private toast = inject(ToastService);
  private i18n = inject(I18nService);

  isSaving = signal(false);

  /** Zones a Philippine store might plausibly pick. */
  readonly timeZones = [
    { id: 'Asia/Manila', label: 'Manila (UTC+8)' },
    { id: 'Asia/Singapore', label: 'Singapore (UTC+8)' },
    { id: 'Asia/Hong_Kong', label: 'Hong Kong (UTC+8)' },
    { id: 'Asia/Dubai', label: 'Dubai (UTC+4)' },
    { id: 'UTC', label: 'UTC' }
  ];

  form = this.fb.group({
    storeName: ['', Validators.required],
    lowStockAlertLimit: [10, [Validators.required, Validators.min(0)]],
    smsAlertTemplate: [''],
    // Posted on every save; leaving it out would reset the stored zone.
    timeZoneId: ['Asia/Manila', Validators.required]
  });

  ngOnInit() {
    this.settingsService.get().subscribe(settings => this.form.patchValue(settings));
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);

    this.settingsService.update(this.form.getRawValue() as StoreSettings).subscribe({
      next: saved => {
        this.isSaving.set(false);
        // The topbar caches the name from login, so refresh it here too.
        this.auth.setStoreName(saved.storeName);
        this.toast.success(this.i18n.translate('settings.saved'));
      },
      error: () => this.isSaving.set(false)
    });
  }
}
