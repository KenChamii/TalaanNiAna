import { DICTIONARY, TRANSLATION_PARTS } from './dictionary';

describe('DICTIONARY', () => {
  const placeholders = (text: string) => [...text.matchAll(/\{(\w+)\}/g)].map(match => match[1]).sort();

  it('defines every key exactly once across the translation files', () => {
    const total = TRANSLATION_PARTS.reduce((count, part) => count + Object.keys(part).length, 0);
    expect(Object.keys(DICTIONARY).length).toBe(total);
  });

  it('has Tagalog and English text for every key', () => {
    for (const [key, entry] of Object.entries(DICTIONARY)) {
      expect(entry.tl.trim()).withContext(`${key} tl`).not.toBe('');
      expect(entry.en.trim()).withContext(`${key} en`).not.toBe('');
    }
  });

  it('uses the same placeholders in both languages', () => {
    for (const [key, entry] of Object.entries(DICTIONARY)) {
      expect(placeholders(entry.tl)).withContext(key).toEqual(placeholders(entry.en));
    }
  });
});
