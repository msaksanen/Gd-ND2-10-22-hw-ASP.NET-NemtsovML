import {MediaMatcher} from '@angular/cdk/layout';
import {ChangeDetectorRef, Component, OnDestroy, OnInit} from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from './services/aurh/auth.service';
import { Userdatamodel } from './models/userdatamodel';
import { User } from './models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'MedContact';
  mobileQuery: MediaQueryList;
  someSubscription: any;
  userPreview: Userdatamodel | undefined| null;



  private _mobileQueryListener: () => void;

  constructor(changeDetectorRef: ChangeDetectorRef,
    private router: Router,
    private authService: AuthService,
    media: MediaMatcher) {
    this.mobileQuery = media.matchMedia('(max-width: 600px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);

    this.router.routeReuseStrategy.shouldReuseRoute = function () {
      return false;
    };
    this.someSubscription = this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.router.navigated = false;
      }
    });
  }


  ngOnInit(): void {
  }

  ngOnDestroy() {
    if (this.someSubscription) {
      this.someSubscription.unsubscribe();
    }
  }
  isLoggedIn(): boolean {
    const user = this.authService.userValue;
    const preview = this.authService.userDataValue;
    if (user){
     this.userPreview= preview;
      return true;
    }

    else
      return false;

  }


}
