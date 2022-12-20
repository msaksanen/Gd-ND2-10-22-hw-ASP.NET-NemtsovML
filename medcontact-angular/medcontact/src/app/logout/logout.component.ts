import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/aurh/auth.service';
import { Location } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss']
})
export class LogoutComponent  implements OnInit {

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.logout();
  }


  logout(): void {
    this.authService.logout();
  }

  goBack() {
    this.router.navigate(['']);
   }

}
