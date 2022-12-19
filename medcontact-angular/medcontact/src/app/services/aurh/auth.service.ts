import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { Userdto } from 'src/app/models/userdto';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject?: BehaviorSubject<Userdto | null>;
  public user?: Observable<Userdto | null>;

  constructor(private router: Router, private apiService: ApiService) {
    this.userSubject = new BehaviorSubject<Userdto | null>(
      JSON.parse(localStorage.getItem('user')!)
    );
    this.user = this.userSubject.asObservable();
  }

  public get userValue() {
    return this.userSubject?.value;
  }

  login(username: string, password: string) {
    return this.apiService.post('Token/CreateJwtToken', { email: username, password }).pipe(
      map((user) => {
        localStorage.setItem('user', JSON.stringify(user));
        this.userSubject?.next(user);
        return user;
      })
    );
  }

  refreshToken(refreshToken: string) {
    return this.apiService.post('Token/RefreshToken', { refreshToken }).pipe(
      map((user) => {
        localStorage.setItem('user', JSON.stringify(user));
        this.userSubject?.next(user);
        return user;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.userSubject?.next(null);
    this.router.navigateByUrl('/login');
  }
}
