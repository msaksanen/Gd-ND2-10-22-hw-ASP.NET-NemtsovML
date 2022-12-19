import { TimetabledoctindexComponent } from './timetabledoctindex/timetabledoctindex.component';
import { HomepageComponent } from './homepage/homepage.component';
import { AppComponent } from './app.component';
import { DoctorsComponent } from './doctors/doctors.component';
import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoctordetailsComponent } from './doctordetails/doctordetails.component';
import { LoginComponent } from './login/login.component';

const routes: Routes = [
  {path:'', component: HomepageComponent, pathMatch:'full'},
  {path: 'doctors', component: DoctorsComponent},
  {path:'timetabledoctindex/:dataid', component:TimetabledoctindexComponent },
  {path:'login', component:LoginComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
