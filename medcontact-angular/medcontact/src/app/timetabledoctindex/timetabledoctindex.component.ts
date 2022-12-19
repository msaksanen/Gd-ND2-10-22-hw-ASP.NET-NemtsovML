import { Sortstate } from './../models/sortstate';
import { Timetabledoctindexmodel } from 'src/app/models/timetabledoctindexmodel';
import { DaytimetableService } from './../services/daytimetable/daytimetable.service';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Daytimetabledto } from '../models/daytimetabledto';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Subscription } from 'rxjs/internal/Subscription';
import { merge, tap } from 'rxjs';
import { Location } from '@angular/common';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { TimetabledoctindexDataSource } from '../datasource/timetabledoctindex-data-source';
import { NIL as NIL_UUID } from 'uuid';
import { MatSort } from '@angular/material/sort';



@Component({
  selector: 'app-timetabledoctindex',
  templateUrl: './timetabledoctindex.component.html',
  styleUrls: ['./timetabledoctindex.component.scss']
})
export class TimetabledoctindexComponent implements OnInit, AfterViewInit  {

    dataSource?: TimetabledoctindexDataSource;
    displayedColumns= ["date", "startWorkTime", "finishWorkTime",  "consultDuration",
     "totalTicketQty", "freeTicketQty", "creationDate", "details"];

    dataid:string='';
    uid?:string;
    reflink?:string;
    page?:number;
    sortorder?:number;
    pageSize?:number;
    model?: Timetabledoctindexmodel;
    private querySubscription?: Subscription;
    private routeSubscription?: Subscription;
    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;


    constructor(private daytimetableService: DaytimetableService,
    private location:Location,
    private route: ActivatedRoute) { }

     ngOnInit() : void{
      this.routeSubscription = this.route.params.subscribe(params=>this.dataid=params['dataid']);
      this.querySubscription = this.route.queryParams.subscribe(
        (queryParam: any) => {
            // this.dataid = queryParam['dataid'];
            this.uid = queryParam['uid'];
            this.reflink = queryParam['reflink'];
            this.page = queryParam['page'];
            this.pageSize = queryParam['pagesize'];
            this.sortorder = queryParam['sortorder'];
        }
      );

      this.daytimetableService.getTimeTableDoctIndexFromApi(this.dataid, this.uid, this.reflink, this.page?.toString(),
                     this.pageSize?.toString(),this.sortorder?.toString())
                    .subscribe(data => this.model = data);

      if (!this.model?.reflink && !this.reflink)
      {
        this.reflink = '/doctors';
      }


      if (!this.model?.user && !this.uid)
        this.uid = NIL_UUID;

      if (!this.model?.sortViewModel?.current && !this.sortorder)
        this.sortorder = 9;


      this.dataSource = new TimetabledoctindexDataSource(this.daytimetableService);
      this.dataSource.loadDaytimetable(this.dataid, this.uid, this.reflink, 1, 3,  this.sortorder);

    //   this.daytimetableService.getLiteDaytimetableModel()
    //  .subscribe(data => this.model = data);

   }

   ngAfterViewInit() {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    if (this.sort.direction==='asc')   this.sortorder=9;   //(desc next time)
    if (this.sort.direction==='desc')  this.sortorder=8;  //(asc next time)
    if (this.model?.sortViewModel!=null && this.sortorder!=null)
       this.model.sortViewModel.current = this.sortorder;

    merge(this.sort.sortChange, this.paginator.page)
       .pipe(
          tap(() => this.loadPage())
       )
       .subscribe();
    }

   loadPage() {
      this.dataSource?.loadDaytimetable(
      this.dataid,
      this.uid, this.reflink,
      this.paginator.pageIndex+1,
      this.paginator.pageSize,
      this.sortorder);
    }


    goBack() {
    this.location.back();
   }
}
