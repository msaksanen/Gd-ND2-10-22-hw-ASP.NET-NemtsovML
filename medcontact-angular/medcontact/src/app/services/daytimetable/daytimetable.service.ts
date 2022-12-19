import { Sortstate } from './../../models/sortstate';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Timetabledoctindexmodel } from 'src/app/models/timetabledoctindexmodel';
import { ApiService } from '../api.service';
import { Daytimetabledto } from 'src/app/models/daytimetabledto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DaytimetableService {

  constructor(private apiService: ApiService) { }

  liteModel?:Timetabledoctindexmodel;

  getTimeTableDoctIndexFromApi(dataid?:string, uid?:string, reflink?:string, page?:string, pagesize?:string, sortorder?:string):
    Observable<Timetabledoctindexmodel>{
      return this.apiService.get('DayTimeTable/TimeTableDoctIndex/',{
            dataid: dataid,
            uid: uid,
            reflink: reflink,
            page: page,
            pageSize: pagesize,
            sortorder: sortorder
            }).pipe();}

  // getTimeTableDoctIndexCollectionFromApi(dataid?:string, uid?:string, reflink?:string, page?:string, pagesize?:string, sortorder?:string):
  //   Observable<Daytimetabledto[]>{
  //    return (this. apiService.get('DayTimeTable/TimeTableDoctIndex/',{
  //           dataid: dataid,
  //           uid: uid,
  //           reflink: reflink,
  //           page: page,
  //           pageSize: pagesize,
  //           sortorder: sortorder
  //           }).pipe() as Observable<Timetabledoctindexmodel>)?.tableList;
  //         }
   getDaytimetabledtoCollectFromApi(dataid?:string, uid?:string, reflink?:string, page?:string, pagesize?:string, sortorder?:string):
    Observable<Daytimetabledto[]>{
      return  this.apiService.get('DayTimeTable/TimeTableDoctIndex/',{
                  dataid: dataid,
                   uid: uid,
                   reflink: reflink,
                   page: page,
                   pageSize: pagesize,
                   sortorder: sortorder
                   }).pipe(map((data:any)=>{
            let ttableDoctIndexmodel = data["tableList"];
               let  model= data as Timetabledoctindexmodel;
               if(model) {
                this.liteModel =model;
                this.getLiteDaytimetableModel();
               }
            return ttableDoctIndexmodel as Daytimetabledto[] ;
              }))};

   getLiteDaytimetableModel() : Observable<Timetabledoctindexmodel|undefined> {
      if(this.liteModel)
      {
        this.liteModel.tableList=undefined;
      }
      return of(this.liteModel);
    }
}
