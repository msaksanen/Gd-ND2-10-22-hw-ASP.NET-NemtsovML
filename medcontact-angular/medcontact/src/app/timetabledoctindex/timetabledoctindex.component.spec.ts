import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimetabledoctindexComponent } from './timetabledoctindex.component';

describe('TimetabledoctindexComponent', () => {
  let component: TimetabledoctindexComponent;
  let fixture: ComponentFixture<TimetabledoctindexComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TimetabledoctindexComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimetabledoctindexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
