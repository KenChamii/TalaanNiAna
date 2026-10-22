import { Translations } from '../translation.model';

export const ERRORS: Translations = {
  'error.offline': { tl: 'Hindi maka-connect sa server. Tingnan ang internet.', en: 'Can’t reach the server. Check your internet connection.' },
  'error.sessionEnded': { tl: 'Tapos na ang session. Mag-login ulit.', en: 'Your session has ended. Please log in again.' },
  'error.tooFast': { tl: 'Masyadong mabilis. Sandali lang bago subukan ulit.', en: 'Too many requests. Wait a moment and try again.' },
  'error.server': { tl: 'May naganap na error sa server. Pakisubukan ulit.', en: 'Something went wrong on the server. Please try again.' },
  'error.generic': { tl: 'May naganap na error. Pakisubukan ulit.', en: 'Something went wrong. Please try again.' },

  'error.code.stock_changed': { tl: 'Nagbago ang stock habang sine-save. Pakisubukan ulit.', en: 'The stock changed while saving. Please try again.' },
  'error.code.future_date': { tl: 'Hindi pa dumarating ang petsang iyon.', en: 'That date hasn’t happened yet.' },
  'error.code.locked_out': { tl: 'Masyadong maraming maling subok. Subukan ulit mamaya.', en: 'Too many failed attempts. Try again later.' },
  'error.code.username_taken': { tl: 'May gumagamit na ng username na ito. Pumili ng iba.', en: 'That username is taken. Please choose another.' },
  'error.code.not_found': { tl: 'Hindi nahanap ang hinahanap mo.', en: 'We couldn’t find that.' },
  'error.code.server_error': { tl: 'May naganap na error sa server. Pakisubukan ulit.', en: 'Something went wrong on the server. Please try again.' }
};
