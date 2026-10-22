import { Translations } from '../translation.model';

export const OFFLINE: Translations = {
  'offline.noConnection': { tl: 'Walang koneksyon. Tuloy lang ang benta.', en: 'No connection. Keep selling.' },
  'offline.sending': { tl: 'Ipinapadala ang mga naunang benta…', en: 'Sending earlier sales…' },
  'offline.waiting': { tl: '{count} naghihintay', en: '{count} waiting' },
  'offline.retry': { tl: 'Subukan ulit ngayon', en: 'Try again now' },
  'offline.discard': { tl: 'Burahin', en: 'Discard' },
  'offline.confirmDiscard': { tl: 'Burahin ang bentang ito? Hindi na ito maitatala.', en: 'Discard this sale? It won’t be recorded.' },
  'offline.queued': { tl: 'Nakatala muna rito. Ipapadala kapag may signal na.', en: 'Saved on this device. It will send once there’s signal.' },
  'offline.nothingWaiting': {
    tl: 'Walang naghihintay na benta. Ang susunod ay itatago muna rito hanggang bumalik ang internet.',
    en: 'Nothing waiting. The next sale will be held here until the connection returns.'
  },
  'offline.rejected': {
    tl: 'Bentang tinanggihan ng server: {count}. Kadalasan, ibig sabihin nito ay naubos na ang stock o nagbago ang presyo. Basahin ang dahilan, saka burahin at ulitin kung kailangan.',
    en: 'Sales rejected by the server: {count}. This usually means an item ran out of stock or its price changed. Read the reason, then discard and ring it up again if needed.'
  }
};
