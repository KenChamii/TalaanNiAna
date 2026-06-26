import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = error.error?.message
        ?? (error.status === 0
              ? 'Hindi maka-connect sa server. Tingnan ang internet connection.'
              : 'May naganap na error. Pakisubukan ulit.');

      // Simple for now — swap for a toast/snackbar service later if you add one.
      alert(message);
      return throwError(() => error);
    })
  );
};