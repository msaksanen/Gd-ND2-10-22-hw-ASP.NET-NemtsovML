import { Daytimetabledto } from './../models/daytimetabledto';
import { DaytimetableService } from './../services/daytimetable/daytimetable.service';
import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject, catchError, Observable, of, finalize  } from "rxjs";
import { NIL as NIL_UUID } from 'uuid';


export class TimetabledoctindexDataSource  implements DataSource<Daytimetabledto> {


  private daytimetableSubject = new BehaviorSubject<Daytimetabledto[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();

  constructor(private daytimetableService: DaytimetableService) {}

  connect(collectionViewer: CollectionViewer): Observable<Daytimetabledto[]> {
    return this.daytimetableSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.daytimetableSubject.complete();
    this.loadingSubject.complete();
  }

  loadDaytimetable(dataid:string, uid= NIL_UUID, reflink='/doctors', page:number=0, pagesize:number=3,
  sortorder=9) {

  this.loadingSubject.next(true);

  this.daytimetableService.getDaytimetabledtoCollectFromApi(dataid, uid, reflink, page.toString(), pagesize.toString(),
        sortorder.toString()).pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
        )
        .subscribe(data => this.daytimetableSubject.next(data));
    }

}
