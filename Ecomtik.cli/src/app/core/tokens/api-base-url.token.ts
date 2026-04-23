import { InjectionToken } from '@angular/core';

/** Пустая строка — относительные пути `/api/...` (dev-прокси). Иначе база без завершающего `/`. */
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL', {
  factory: () => ''
});
