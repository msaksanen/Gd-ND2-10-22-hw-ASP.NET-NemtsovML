import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorpreviewComponent } from './doctorpreview.component';

describe('DoctorpreviewComponent', () => {
  let component: DoctorpreviewComponent;
  let fixture: ComponentFixture<DoctorpreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DoctorpreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorpreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
