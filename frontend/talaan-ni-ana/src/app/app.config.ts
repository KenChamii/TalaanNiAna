import { APP_INITIALIZER, ApplicationConfig, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { routes } from './app.routes';
import { AuthService } from './core/auth/auth.service';
import { jwtInterceptor } from './core/interceptors/jwt.interceptors';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { provideServiceWorker } from '@angular/service-worker';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

/** The access token lives in memory, so a reload restores it from the refresh cookie. */
function restoreSession(auth: AuthService) {
  return () => firstValueFrom(auth.restoreSession());
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    // ngx-charts animates with @angular/animations; loaded lazily so the landing page doesn't pay for it.
    provideAnimationsAsync(),
    // jwt runs first so it can retry a 401; error runs outside it and only sees failures.
    provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor])),
    {
      provide: APP_INITIALIZER,
      useFactory: restoreSession,
      deps: [AuthService],
      multi: true
    }, provideServiceWorker('ngsw-worker.js', {
            enabled: !isDevMode(),
            registrationStrategy: 'registerWhenStable:30000'
          })
  ]
};
