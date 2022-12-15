import { Doctdataindexviewmodel } from './../../models/doctdataindexviewmodel';
import { ApiService } from './../api.service';
import { Doctorfulldata } from './../../models/doctorfulldata';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import {HttpClient, HttpRequest, HttpResponse} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DoctorsService {

  // apiUrl: string = 'https://localhost:7003/';
  // constructor(private httpClient: HttpClient) { }
  constructor(private apiService: ApiService) { }

  // getDoctors() : Observable<Doctorfulldata[]> {
  //   let data = [
  //     {
  //       Id: '5B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       UserId: '6B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       Username: 'Mikas',
  //       Surname: 'Saxen',
  //       MidName: 'Leo',
  //       Email: 'wisor@gmail.com',
  //       BirthDate: new Date(),
  //       Gender: 'male',
  //       IsBlocked: false,
  //       IsFullBlocked: false,
  //       ForDeletion: false,
  //       SpecialityId: '5B383A46-AF4E-484A-B515-DBF8E16926E3',
  //       SpecialityName: 'Cardiology',
  //       SpecNameReserved: 'Cardiology',
  //       Roles: [{ guidId : '5E383A46-AF4E-484A-B515-DBF8E16926E3',Name:'Customer'},
  //               { guidId : '5E383A47-AF4E-484A-B515-DBF8E16926E3',Name:'Doctor'}]
  //     },
  //      {
  //       Id: '6B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       UserId: '6B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       Username: 'Will',
  //       Surname: 'Saxen',
  //       MidName: 'Leo',
  //       Email: 'wisor@gmail.com',
  //       BirthDate: new Date(),
  //       Gender: 'male',
  //       IsBlocked: false,
  //       IsFullBlocked: false,
  //       ForDeletion: false,
  //       SpecialityId: '5B383A46-AF4E-484A-B515-DBF8E16926E3',
  //       SpecialityName: 'Cardiology',
  //       SpecNameReserved: 'Cardiology',
  //       Roles: [{ guidId : '5E383A46-AF4E-484A-B515-DBF8E16926E3',Name:'Customer'},
  //               { guidId : '5E383A47-AF4E-484A-B515-DBF8E16926E3',Name:'Doctor'}]
  //     },
  //     {
  //       Id: '7B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       UserId: '6B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       Username: 'Jane',
  //       Surname: 'Saxen',
  //       MidName: 'Leo',
  //       Email: 'wisor@gmail.com',
  //       BirthDate: new Date(),
  //       Gender: 'male',
  //       IsBlocked: false,
  //       IsFullBlocked: false,
  //       ForDeletion: false,
  //       SpecialityId: '5B383A46-AF4E-484A-B515-DBF8E16926E3',
  //       SpecialityName: 'Pulmonology',
  //       SpecNameReserved: 'Pulmonology',
  //       Roles: [{ guidId : '5E383A46-AF4E-484A-B515-DBF8E16926E3',Name:'User'},
  //               { guidId : '5E383A47-AF4E-484A-B515-DBF8E16926E3',Name:'Doctor'}]
  //     },
  //     {Id: '8B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       UserId: '6B383A46-AF4E-484A-B515-DBF8E16926E2',
  //       Username: 'Jack',
  //       Surname: 'Saxen',
  //       MidName: 'Leo',
  //       Email: 'wisor@gmail.com',
  //       BirthDate: new Date() ,
  //       Gender: 'male',
  //       IsBlocked: false,
  //       IsFullBlocked: false,
  //       ForDeletion: false,
  //       SpecialityId: '5B383A46-AF4E-484A-B515-DBF8E16926E3',
  //       SpecialityName: 'Pulmonology',
  //       SpecNameReserved: 'Pulmonology',
  //       Roles: [{ guidId : '5E383A46-AF4E-484A-B515-DBF8E16926E3',Name:'Customer'},
  //               { guidId : '5E383A47-AF4E-484A-B515-DBF8E16926E3',Name:'Admin'}]}
  //   ]
  //   return of (data);
  // }

  // getAllDoctorsFromApi(): Observable<Doctorfulldata[]>{
  //   let finalUrl : string = this.apiUrl + 'api/Doctor/Index/';
  //   return this.httpClient.get<Doctorfulldata[]>(finalUrl);
  // }
  // getAllDoctorsFromApi(): Observable<Doctorfulldata[]>{
  //   return this.apiService.get('Doctor/Index/',{}).pipe();
    getAllDoctorsFromApi(): Observable<Doctdataindexviewmodel>{
      return this.apiService.get('Doctor/Index/',{}).pipe();
  }

}
