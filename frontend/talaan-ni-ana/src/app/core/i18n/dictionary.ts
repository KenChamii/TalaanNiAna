import { Translations } from './translation.model';
import { COMMON } from './translations/common';
import { LANDING } from './translations/landing';
import { AUTH } from './translations/auth';
import { POS } from './translations/pos';
import { OFFLINE } from './translations/offline';
import { DASHBOARD } from './translations/dashboard';
import { INVENTORY } from './translations/inventory';
import { STOCK } from './translations/stock';
import { CUSTOMERS } from './translations/customers';
import { TRANSACTIONS } from './translations/transactions';
import { REPORTS } from './translations/reports';
import { SETTINGS } from './translations/settings';
import { ERRORS } from './translations/errors';

export const TRANSLATION_PARTS: Translations[] = [
  COMMON, LANDING, AUTH, POS, OFFLINE, DASHBOARD, INVENTORY,
  STOCK, CUSTOMERS, TRANSACTIONS, REPORTS, SETTINGS, ERRORS
];

export const DICTIONARY: Translations = Object.assign({}, ...TRANSLATION_PARTS);
