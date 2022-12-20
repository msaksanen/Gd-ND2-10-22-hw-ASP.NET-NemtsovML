import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/aurh/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private router: Router,
  private authService: AuthService){}

canActivate(
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const user = this.authService.userValue;

    // user?.roles?.some(role => role.name==='Admin'

    if (user?.roles?.includes('Admin')) {
      return true;
    }

    this.router.navigate(['/login'], { queryParams: {returnUrl: state.url} });
    return false;
}

}
