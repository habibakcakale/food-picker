import {Component, OnInit} from '@angular/core';
import {OrderDateService} from "./order-date.service";

@Component({
  selector: 'app-order-date-picker',
  templateUrl: './order-date-picker.component.html',
  styleUrls: ['./order-date-picker.component.scss']
})
export class OrderDatePickerComponent implements OnInit {
  selectedDate: Date;

  constructor(private dateService: OrderDateService) {
  }

  ngOnInit(): void {
  }

  dateChanged($event: Date) {
    this.dateService.emit($event);
  }
}
