import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../../shared/services/toast.service';
import { SILENT_404 } from './silent-404.token';

/** RFC 7807 payload the API returns for every handled error. */
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

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const problem: ProblemDetails = error.error ?? {};

      if (error.status === 0) {
        toast.error('Hindi maka-connect sa server. Tingnan ang internet.');
        return throwError(() => error);
      }

      // The jwt interceptor already tried to refresh, so a 401 here is terminal.
      if (error.status === 401) {
        if (!req.url.includes('/auth/')) {
          toast.warning('Tapos na ang session. Mag-login ulit.');
          router.navigate(['/login']);
        }
        return throwError(() => error);
      }

      if (error.status === 429) {
        toast.warning('Masyadong mabilis. Sandali lang bago subukan ulit.');
        return throwError(() => error);
      }

      // A lookup that found nothing is the caller's business, not a failure.
      if (error.status === 404 && req.context.get(SILENT_404)) {
        return throwError(() => error);
      }

      toast.error(messageFor(problem, error.status));
      return throwError(() => error);
    })
  );
};

function messageFor(problem: ProblemDetails, status: number): string {
  // Field errors are more useful than the summary, so show the first one.
  const firstFieldError = problem.errors
    ? Object.values(problem.errors).flat().find(Boolean)
    : undefined;

  if (firstFieldError) return firstFieldError;
  if (problem.detail) return problem.detail;
  if (status >= 500) return 'May naganap na error sa server. Pakisubukan ulit.';

  return 'May naganap na error. Pakisubukan ulit.';
}
