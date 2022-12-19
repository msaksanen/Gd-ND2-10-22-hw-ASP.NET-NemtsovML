import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { AuthService } from '../services/aurh/auth.service';

@Injectable()
export class AuthErrorInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((err) => {
        if ([401, 403].includes(err.status)) {
          //if use refresh token -> logic of refresh should be there
          this.authService.logout();
        }

        const error = err.error.message || err.statusText;
        return throwError(() => error);
      })
    );
  }
}
