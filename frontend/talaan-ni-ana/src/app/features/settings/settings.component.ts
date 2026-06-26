import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { SettingsService } from './settings.service';

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

  isSaving = signal(false);
  savedMessage = signal<string | null>(null);

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
    this.savedMessage.set(null);
    this.settingsService.update(this.form.value).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.savedMessage.set('Na-save ang settings!');
      },
      error: () => this.isSaving.set(false)
    });
  }
}