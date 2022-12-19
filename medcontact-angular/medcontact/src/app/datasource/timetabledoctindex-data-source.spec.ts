// import { DaytimetableService } from '../services/daytimetable/daytimetable.service';
// //import { TimetabledoctindexDataSource } from './timetabledoctindex-data-source';

// daytimetableService:DaytimetableService;
// describe('TimetabledoctindexDataSource', () => {
//   it('should create an instance', () => {
//     expect(new TimetabledoctindexDataSource(DaytimetableService)).toBeTruthy();
//   });
// });


import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimetabledoctindexDataSource } from './timetabledoctindex-data-source';

describe('HomepageComponent', () => {
  let component: TimetabledoctindexDataSource;
  let fixture: ComponentFixture<TimetabledoctindexDataSource>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TimetabledoctindexDataSource ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimetabledoctindexDataSource);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
