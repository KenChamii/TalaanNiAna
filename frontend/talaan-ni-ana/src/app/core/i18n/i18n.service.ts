import { Injectable, computed, signal } from '@angular/core';
import { DICTIONARY } from './dictionary';
import { TranslationParams } from './translation.model';

export type Language = 'tl' | 'en';

const STORAGE_KEY = 'talaan_lang';

@Injectable({ providedIn: 'root' })
export class I18nService {
  readonly language = signal<Language>(this.read());

  readonly locale = computed(() => this.language() === 'en' ? 'en-PH' : 'fil-PH');

  /** Reads the saved language, defaulting to Tagalog when storage is unavailable. */
  private read(): Language {
    try {
      return localStorage.getItem(STORAGE_KEY) === 'en' ? 'en' : 'tl';
    } catch {
      return 'tl';
    }
  }

  /** Switches between Tagalog and English. */
  toggle() {
    this.use(this.language() === 'tl' ? 'en' : 'tl');
  }

  /** Applies a language and remembers it when storage allows. */
  use(language: Language) {
    this.language.set(language);
    document.documentElement.lang = language === 'tl' ? 'fil' : 'en';

    try {
      localStorage.setItem(STORAGE_KEY, language);
    } catch {
      return;
    }
  }

  /** Whether the dictionary has an entry for the key. */
  has(key: string): boolean {
    return key in DICTIONARY;
  }

  /** Returns the text for the key in the current language, filling in any {placeholders}. */
  translate(key: string, params?: TranslationParams): string {
    const entry = DICTIONARY[key];
    if (!entry) return key;

    const text = this.language() === 'en' ? entry.en : entry.tl;
    if (!params) return text;

    return text.replace(/\{(\w+)\}/g, (match, name: string) =>
      name in params ? String(params[name] ?? '') : match);
  }
}
