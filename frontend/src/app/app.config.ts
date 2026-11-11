import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';
import { provideHttpClient } from '@angular/common/http';
import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';

registerLocaleData(localeEs);

// 10-11-2026 Construido por: Ing GAAE - Application providers including the HTTP client and the Spanish locale.
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    { provide: LOCALE_ID, useValue: 'es' }
  ]
};
