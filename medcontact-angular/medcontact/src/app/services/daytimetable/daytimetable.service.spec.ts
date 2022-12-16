import { TestBed } from '@angular/core/testing';

import { DaytimetableService } from './daytimetable.service';

describe('DaytimetableService', () => {
  let service: DaytimetableService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DaytimetableService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
