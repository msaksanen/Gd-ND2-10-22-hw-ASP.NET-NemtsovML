import { Sortstate } from './../../models/sortstate';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Timetabledoctindexmodel } from 'src/app/models/timetabledoctindexmodel';
import { ApiService } from '../api.service';

@Injectable({
  providedIn: 'root'
})
export class DaytimetableService {

  constructor(private apiService: ApiService) { }

  getTimeTableDoctIndexFromApi(dataid?:string, uid?:string, reflink?:string, page?:string, sortorder?:Sortstate):
    Observable<Timetabledoctindexmodel>{
      return this.apiService.get('DayTimeTable/TimeTableDoctIndex/',{
            dataid: dataid,
            uid: uid,
            reflink: reflink,
            page: page,
            sortorder: sortorder
            }).pipe();}


//     getOnlinerArticlesFromApi(): Observable<Article[]>{
//       return this.apiService.get('Articles', {
//         sourceId: "02cff32a-097a-48b4-8240-f27f81e8a0c1",
//         pageSize:5,
//         pageNumber:0}).pipe();
// getArticleByIdFromApi(id:string): Observable<Article>{
//   return this.apiService.get(`Articles/${id}`, {}).pipe();

}
