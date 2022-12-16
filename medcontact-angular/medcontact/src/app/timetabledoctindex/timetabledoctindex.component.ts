import { Timetabledoctindexmodel } from 'src/app/models/timetabledoctindexmodel';
import { DaytimetableService } from './../services/daytimetable/daytimetable.service';
import { Component, OnInit } from '@angular/core';
import { Sortstate } from '../models/sortstate';
import { Daytimetabledto } from '../models/daytimetabledto';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-timetabledoctindex',
  templateUrl: './timetabledoctindex.component.html',
  styleUrls: ['./timetabledoctindex.component.scss']
})
export class TimetabledoctindexComponent implements OnInit  {

  model?: Timetabledoctindexmodel;
  dataid?:string;
  uid?:string;
  reflink?:string;
  page?:string;
  sortorder?:Sortstate;
  item?: Daytimetabledto;

  constructor (private daytimetableService: DaytimetableService,
               private route: ActivatedRoute) {

  }
  ngOnInit(): void {
    /*  this.doctorsService.getDoctors()
     .subscribe(data => this.doctors = data); */
     const dataidConst  = this.route.snapshot.paramMap.get('dataid') || '';
     this.dataid = dataidConst;
     this.daytimetableService.getTimeTableDoctIndexFromApi(this.dataid, this.uid, this.reflink, this.page,this.sortorder)
     .subscribe(data => this.model = data);
   }

}
