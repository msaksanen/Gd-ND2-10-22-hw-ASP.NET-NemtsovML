import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, map, forkJoin, mergeMap } from 'rxjs';
import { User } from 'src/app/models/user';
import { Userdatamodel } from 'src/app/models/userdatamodel';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject?: BehaviorSubject<User | null>;
  public user?: Observable<User | null>;

  private userdataSubject?: BehaviorSubject<Userdatamodel | null>;
  public userdata?: Observable<Userdatamodel | null>;

  private isloggedInSubject = new BehaviorSubject<boolean>(false);

  constructor(private router: Router, private apiService: ApiService) {
    this.userSubject = new BehaviorSubject<User | null>(
      JSON.parse(localStorage.getItem('user')!)
    );
    this.user = this.userSubject.asObservable();
    this.userdataSubject = new BehaviorSubject<Userdatamodel | null>(
      JSON.parse(localStorage.getItem('userdata')!)
    );
    this.userdata = this.userdataSubject.asObservable();
  }

  public get userValue() {
    return this.userSubject?.value;
  }

  public get userData() {
    return this.userdataSubject?.asObservable();
  }

  public get isloggedIn() {
    return this.isloggedInSubject?.asObservable();
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

  getUserLoginPreview(){
    return this.apiService.get('Account/UserLoginPreview',{}).pipe(
      map((data) => {
        this.isloggedInSubject.next(true);
        localStorage.setItem('userdata', JSON.stringify(data));
        this.userdataSubject?.next(data);
        return data;
      })
    );
  }

  combiLogin(username:string, password:string) {
    return this.login(username, password).pipe((mergeMap(user =>(this.getUserLoginPreview()))));
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
    localStorage.removeItem('userdata');
    this.userdataSubject?.next(null);
    this.isloggedInSubject.next(false);
   // this.router.navigateByUrl('/logout');
  }
}
