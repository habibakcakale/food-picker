import {Injectable, EventEmitter} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class OrderDateService {
  private eventEmitter = new EventEmitter<Date>()


  constructor() {
  }

  emit($event: Date) {
    this.eventEmitter.emit($event);
  }

  subscribe(fn) {
    return this.eventEmitter.subscribe(fn);
  }
}
