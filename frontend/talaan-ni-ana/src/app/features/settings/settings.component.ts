import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { SettingsService } from './settings.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private settingsService = inject(SettingsService);
  private toast = inject(ToastService);

  isSaving = signal(false);

  form = this.fb.group({
    storeName: [''],
    lowStockAlertLimit: [10],
    smsAlertTemplate: ['']
  });

  ngOnInit() {
    this.settingsService.get().subscribe(settings => this.form.patchValue(settings));
  }

  submit() {
    this.isSaving.set(true);
    this.settingsService.update(this.form.value).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.toast.success('Na-save ang settings!');
      },
      error: () => {
        this.isSaving.set(false);
        this.toast.error('Hindi na-save ang settings. Subukan muli.');
      }
    });
  }
}