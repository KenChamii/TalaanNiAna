import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConnectionService } from '../../../core/offline/connection.service';
import { OutboxService } from '../../../core/offline/outbox.service';
import { PesoPipe } from '../../pipes/peso.pipes';

@Component({
  selector: 'app-offline-banner',
  standalone: true,
  imports: [CommonModule, PesoPipe],
  templateUrl: './offline-banner.component.html',
  styleUrl: './offline-banner.component.scss'
})
export class OfflineBannerComponent {
  readonly connection = inject(ConnectionService);
  readonly outbox = inject(OutboxService);

  isExpanded = signal(false);

  toggle() {
    this.isExpanded.update(open => !open);
  }

  retry() {
    void this.outbox.flush();
  }

  discard(id: number | undefined) {
    if (id === undefined) return;
    if (!confirm('Burahin ang bentang ito? Hindi na ito maitatala.')) return;

    void this.outbox.discard(id);
  }
}
