import { TestBed } from '@angular/core/testing';

import { OrderDateService } from './order-date.service';

describe('OrderDateService', () => {
  let service: OrderDateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrderDateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
