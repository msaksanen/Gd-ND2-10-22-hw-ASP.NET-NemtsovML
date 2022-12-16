import { Doctdataindexviewmodel } from './../models/doctdataindexviewmodel';
import { Doctorfulldata } from './../models/doctorfulldata';
import { Component, OnInit } from '@angular/core';
import { Doctorslist } from 'src/app/data/doctorslist';
import { DoctorsService } from 'src/app/services/doctors/doctors.service';

@Component({
  selector: 'app-doctors',
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.scss'],
})
export class DoctorsComponent implements OnInit {
  // doctors: Doctorfulldata[] = [];
  doctors?: Doctdataindexviewmodel
  selectedDoctor? : Doctorfulldata;

 constructor (private doctorsService: DoctorsService ){

 }
  ngOnInit(): void {
   /*  this.doctorsService.getDoctors()
    .subscribe(data => this.doctors = data); */
    this.doctorsService.getAllDoctorsFromApi()
    .subscribe(data => this.doctors = data);
  }

  viewDoctorParent(doctor: Doctorfulldata) {
    this.selectedDoctor = doctor;
  }

}
