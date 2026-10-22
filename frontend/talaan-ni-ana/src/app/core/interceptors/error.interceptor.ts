import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../../shared/services/toast.service';
import { I18nService } from '../i18n/i18n.service';
import { SILENT_404 } from './silent-404.token';

interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  code?: string;
  errors?: Record<string, string[]>;
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const router = inject(Router);
  const i18n = inject(I18nService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const problem: ProblemDetails = error.error ?? {};

      if (error.status === 0) {
        toast.error(i18n.translate('error.offline'));
        return throwError(() => error);
      }

      if (error.status === 401) {
        if (!req.url.includes('/auth/')) {
          toast.warning(i18n.translate('error.sessionEnded'));
          router.navigate(['/login']);
        }
        return throwError(() => error);
      }

      if (error.status === 429) {
        toast.warning(i18n.translate('error.tooFast'));
        return throwError(() => error);
      }

      if (error.status === 404 && req.context.get(SILENT_404)) {
        return throwError(() => error);
      }

      toast.error(messageFor(problem, error.status, i18n));
      return throwError(() => error);
    })
  );
};

/** Picks the most useful message: a translated error code, then the server's own text, then a generic one. */
function messageFor(problem: ProblemDetails, status: number, i18n: I18nService): string {
  const codeKey = `error.code.${problem.code}`;
  if (problem.code && i18n.has(codeKey)) return i18n.translate(codeKey);

  const firstFieldError = problem.errors
    ? Object.values(problem.errors).flat().find(Boolean)
    : undefined;

  if (firstFieldError) return firstFieldError;
  if (problem.detail) return problem.detail;

  return i18n.translate(status >= 500 ? 'error.server' : 'error.generic');
}
