import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AuthService } from '../services/aurh/auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    const user = this.authService.userValue;
    const isLoggedIn = user?.accessToken;
    const isApiUrl = request.url.startsWith(environment.apiURL);

    if (isApiUrl && isLoggedIn) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${user.accessToken}`
        }
      })
    }

    return next.handle(request);
  }
}
