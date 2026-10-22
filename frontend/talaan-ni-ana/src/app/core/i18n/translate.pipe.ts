import { Pipe, PipeTransform, inject } from '@angular/core';
import { I18nService } from './i18n.service';
import { TranslationParams } from './translation.model';

@Pipe({ name: 't', standalone: true, pure: false })
export class TranslatePipe implements PipeTransform {
  private i18n = inject(I18nService);

  /** Impure so the text follows the language signal, which is not an argument. */
  transform(key: string, params?: TranslationParams): string {
    return this.i18n.translate(key, params);
  }
}
