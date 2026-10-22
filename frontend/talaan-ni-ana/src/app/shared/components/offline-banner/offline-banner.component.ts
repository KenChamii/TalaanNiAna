import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConnectionService } from '../../../core/offline/connection.service';
import { OutboxService } from '../../../core/offline/outbox.service';
import { I18nService } from '../../../core/i18n/i18n.service';
import { TranslatePipe } from '../../../core/i18n/translate.pipe';
import { PesoPipe } from '../../pipes/peso.pipes';

@Component({
  selector: 'app-offline-banner',
  standalone: true,
  imports: [CommonModule, PesoPipe, TranslatePipe],
  templateUrl: './offline-banner.component.html',
  styleUrl: './offline-banner.component.scss'
})
export class OfflineBannerComponent {
  readonly connection = inject(ConnectionService);
  readonly outbox = inject(OutboxService);
  private i18n = inject(I18nService);

  isExpanded = signal(false);

  toggle() {
    this.isExpanded.update(open => !open);
  }

  retry() {
    void this.outbox.flush();
  }

  /** Drops a queued sale after the owner confirms it should never be sent. */
  discard(id: number | undefined) {
    if (id === undefined) return;
    if (!confirm(this.i18n.translate('offline.confirmDiscard'))) return;

    void this.outbox.discard(id);
  }
}
