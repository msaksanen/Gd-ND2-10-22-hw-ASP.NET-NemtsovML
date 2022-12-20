import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../services/aurh/auth.service';
import { first, forkJoin } from 'rxjs';
import { Location } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit{

  loginForm!: FormGroup;
  isHidePassword: boolean = true;
  isSubmitted: boolean = false;
  error?: string;

  constructor(
    private formBuilder: FormBuilder,
    private location: Location,
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService)

    {
      if (this.authService.userValue){
        this.router.navigate(['/']);
      }
    }

     ngOnInit(): void {
      this.loginForm = this.formBuilder.group({
        username: ['', Validators.required],
        password: ['', Validators.required],
      });
    }


     goBack() {
     this.location.back();
     }

     onSubmit(){
      this.isSubmitted = true;

          if (this.loginForm.invalid){
        this.isSubmitted=false;
        return;
      }

      this.error = '';
      const formData = this.loginForm.controls;



      this.authService.combiLogin(this.loginForm.controls['username'].value, this.loginForm.controls['password'].value)
        .pipe(first())
        .subscribe({
          next: () => {
            const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
            this.router.navigateByUrl(returnUrl);
          },
          error: error  => {
            this.error = error;
            this.isSubmitted = false;
          }
        });



    }
}
