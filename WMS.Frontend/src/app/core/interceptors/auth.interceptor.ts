import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const currentUser = authService.currentUser();
  const isAuthEndpoint = req.url.includes('/auth/login') || req.url.includes('/auth/change-password');

  if (currentUser && currentUser.token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${currentUser.token}`
      }
    });
  }

  return next(req).pipe(
    catchError(error => {
      if (!isAuthEndpoint && error?.status === 401) {
        authService.logout(false);
        router.navigate(['/login'], { queryParams: { sessionExpired: true } });
      }

      return throwError(() => error);
    })
  );
};
