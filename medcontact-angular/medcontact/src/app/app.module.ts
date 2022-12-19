import { DoctorpreviewComponent } from './doctorpreview/doctorpreview.component';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DoctorsComponent } from './doctors/doctors.component';
import { DoctordetailsComponent } from './doctordetails/doctordetails.component';
import { HttpClientModule } from '@angular/common/http';
import { HomepageComponent } from './homepage/homepage.component';
import { TimetabledoctindexComponent } from './timetabledoctindex/timetabledoctindex.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatCardModule} from '@angular/material/card';
import { MatDividerModule} from '@angular/material/divider';
import { MatButtonModule, MatIconButton} from '@angular/material/button';
import { MatIconModule} from '@angular/material/icon';
import { MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { MatToolbarModule} from '@angular/material/toolbar';
import { MatListModule} from '@angular/material/list';
import { MatSidenavModule} from '@angular/material/sidenav';
import { MatPaginatorModule} from '@angular/material/paginator';
import { MatInputModule} from '@angular/material/input';
import { MatTableModule} from '@angular/material/table';
import { MatSortModule} from '@angular/material/sort';
import { LoginComponent } from './login/login.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule} from '@angular/material/progress-bar';

@NgModule({
  declarations: [
    AppComponent,
    DoctorsComponent,
    DoctorpreviewComponent,
    DoctordetailsComponent,
    HomepageComponent,
    TimetabledoctindexComponent,
    LoginComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatCardModule,
    MatDividerModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    MatListModule,
    MatSidenavModule,
    MatPaginatorModule,
    MatInputModule,
    MatSortModule,
    MatTableModule,
    MatFormFieldModule,
    MatProgressBarModule,
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
