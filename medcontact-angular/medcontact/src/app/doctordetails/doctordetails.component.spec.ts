import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctordetailsComponent } from './doctordetails.component';

describe('DoctordetailsComponent', () => {
  let component: DoctordetailsComponent;
  let fixture: ComponentFixture<DoctordetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DoctordetailsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctordetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
