import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

/** Shared across concurrent 401s so one refresh serves every queued request. */
let refreshing = false;
const refreshed = new BehaviorSubject<string | null>(null);

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  const authed = withToken(req, auth.getToken());

  return next(authed).pipe(
    catchError((error: HttpErrorResponse) => {
      const retryable = error.status === 401 && !isAuthEndpoint(req.url);

      if (!retryable) return throwError(() => error);

      if (refreshing) {
        // Wait for the in-flight refresh, then replay with the new token.
        return refreshed.pipe(
          filter(token => token !== null),
          take(1),
          switchMap(token => next(withToken(req, token)))
        );
      }

      refreshing = true;
      refreshed.next(null);

      return auth.refresh().pipe(
        switchMap(result => {
          refreshing = false;

          if (!result) return throwError(() => error);

          refreshed.next(result.accessToken);
          return next(withToken(req, result.accessToken));
        }),
        catchError(refreshError => {
          refreshing = false;
          return throwError(() => refreshError);
        })
      );
    })
  );
};

function withToken(req: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
  return token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
}

function isAuthEndpoint(url: string): boolean {
  return url.includes('/auth/');
}
