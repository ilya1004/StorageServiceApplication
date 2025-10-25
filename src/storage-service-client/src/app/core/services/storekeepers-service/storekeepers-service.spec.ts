import { TestBed } from '@angular/core/testing';

import { StorekeepersService } from './storekeepers-service';

describe('StorekeepersService', () => {
  let service: StorekeepersService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StorekeepersService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
