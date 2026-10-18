import { Injectable, signal } from '@angular/core';
import { DICTIONARY } from './dictionary';

export type Language = 'tl' | 'en';

const STORAGE_KEY = 'talaan_lang';

/**
 * Runtime translation, rather than Angular's build-time i18n.
 *
 * Build-time i18n produces one bundle per locale and needs the server to pick
 * between them, which would mean two deployments of a free-tier site. A shop
 * owner switching to English for a visitor wants it to happen on the spot, not
 * on the next page load from a different origin.
 *
 * The cost is that translation happens on every change detection pass, which
 * for an app this size is not measurable.
 */
@Injectable({ providedIn: 'root' })
export class I18nService {
  readonly language = signal<Language>(this.read());

  /** Tagalog is the default: the people behind the counter are the users. */
  private read(): Language {
    try {
      return localStorage.getItem(STORAGE_KEY) === 'en' ? 'en' : 'tl';
    } catch {
      // Private browsing, or storage disabled. Not worth failing over.
      return 'tl';
    }
  }

  toggle() {
    this.use(this.language() === 'tl' ? 'en' : 'tl');
  }

  use(language: Language) {
    this.language.set(language);
    document.documentElement.lang = language === 'tl' ? 'fil' : 'en';

    try {
      localStorage.setItem(STORAGE_KEY, language);
    } catch {
      // The choice simply will not persist. The app still works.
    }
  }

  /**
   * An unknown key returns itself rather than an empty space, so a missed
   * string looks obviously wrong in review instead of disappearing in use.
   */
  translate(key: string): string {
    const entry = DICTIONARY[key];
    if (!entry) return key;

    return this.language() === 'en' ? entry.en : entry.tl;
  }
}
