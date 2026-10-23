import { APP_INITIALIZER, ApplicationConfig, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { AuthService } from './core/auth/auth.service';
import { jwtInterceptor } from './core/interceptors/jwt.interceptors';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { provideServiceWorker } from '@angular/service-worker';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

/** Starts the session restore and API warm-up without holding the first render back. */
function startSession(auth: AuthService) {
  return () => {
    auth.warmUp();
    auth.restoreSession().subscribe();
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    // ngx-charts animates with @angular/animations; loaded lazily so the landing page doesn't pay for it.
    provideAnimationsAsync(),
    // jwt runs first so it can retry a 401; error runs outside it and only sees failures.
    provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor])),
    { provide: APP_INITIALIZER, useFactory: startSession, deps: [AuthService], multi: true },
    provideServiceWorker('ngsw-worker.js', {
            enabled: !isDevMode(),
            registrationStrategy: 'registerWhenStable:30000'
          })
  ]
};
