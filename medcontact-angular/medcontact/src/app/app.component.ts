import {MediaMatcher} from '@angular/cdk/layout';
import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import { AuthService } from './services/aurh/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'MedContact';
  mobileQuery: MediaQueryList;
  isLoggedIn?: string;
  private _mobileQueryListener: () => void;

  constructor(changeDetectorRef: ChangeDetectorRef,
    private authService: AuthService,
    media: MediaMatcher) {
    this.mobileQuery = media.matchMedia('(max-width: 600px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  //need to be reworked
  logout(): void {
    this.authService.logout();
  }
}
