import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, throwError, switchMap, filter, take} from 'rxjs';
import { AuthService } from '../services/aurh/auth.service';
import { environment } from 'src/environments/environment';

@Injectable()
export class AuthErrorInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler)
           :Observable<HttpEvent<unknown>> {
    let authReq = request;
    const token = this.authService.userValue?.accessToken;
    if (token != null) {
       authReq = this.addTokenHeader(authReq, token);
     }

    return next.handle(authReq).pipe(
      catchError((err) => {
        if (err instanceof HttpErrorResponse && [401, 403].includes(err.status)
            && !authReq.url.includes('Token/CreateJwtToken')) {
          //this.authService.logout();
          return this.handleError(authReq, next);
        }

        const error = err.error.message || err.statusText;
        return throwError(() => error);
      })
    );
  }

  private handleError(request: HttpRequest<any>, next: HttpHandler) :
          Observable<HttpEvent<unknown>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const rtoken = this.authService.userValue?.refreshToken;

      if (rtoken)
        return this.authService.refreshToken(rtoken).pipe(
          switchMap((token: any) => {
            this.isRefreshing = false;
            this.refreshTokenSubject.next(token.accessToken);

            return next.handle(this.addTokenHeader(request, token.accessToken));
          }),
          catchError((error) => {
            this.isRefreshing = false;

            this.authService.logout();
            return throwError(() => error);
          })
        );
    }

    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request, token)))
    );
  }

  private addTokenHeader(request: HttpRequest<any>, token: string) {
    // const user = this.authService.userValue;
    // const isLoggedIn = user?.accessToken;
    const isApiUrl = request.url.startsWith(environment.apiURL);

    if (isApiUrl && token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })
    }
    return request;
  }


}
