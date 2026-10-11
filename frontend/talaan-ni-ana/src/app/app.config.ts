import { APP_INITIALIZER, ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { routes } from './app.routes';
import { AuthService } from './core/auth/auth.service';
import { jwtInterceptor } from './core/interceptors/jwt.interceptors';
import { errorInterceptor } from './core/interceptors/error.interceptor';

/** The access token lives in memory, so a reload restores it from the refresh cookie. */
function restoreSession(auth: AuthService) {
  return () => firstValueFrom(auth.restoreSession());
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    // jwt runs first so it can retry a 401; error runs outside it and only sees failures.
    provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor])),
    {
      provide: APP_INITIALIZER,
      useFactory: restoreSession,
      deps: [AuthService],
      multi: true
    }
  ]
};
