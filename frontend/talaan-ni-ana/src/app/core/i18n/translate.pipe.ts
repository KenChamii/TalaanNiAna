import { Pipe, PipeTransform, inject } from '@angular/core';
import { I18nService } from './i18n.service';

/**
 * Impure on purpose: the output depends on the chosen language, which is not
 * one of the pipe's arguments, so a pure pipe would keep showing the language
 * that was active when the view was first rendered.
 */
@Pipe({ name: 't', standalone: true, pure: false })
export class TranslatePipe implements PipeTransform {
  private i18n = inject(I18nService);

  transform(key: string): string {
    return this.i18n.translate(key);
  }
}
