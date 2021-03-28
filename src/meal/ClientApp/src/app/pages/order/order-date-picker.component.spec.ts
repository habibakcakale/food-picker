import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderDatePickerComponent } from './order-date-picker.component';

describe('OrderDatePickerComponent', () => {
  let component: OrderDatePickerComponent;
  let fixture: ComponentFixture<OrderDatePickerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderDatePickerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderDatePickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
