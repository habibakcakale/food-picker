import {Component, OnInit} from '@angular/core';
import {OrderToolBarService} from "./order-tool-bar.service";

@Component({
    selector: 'app-order-date-picker',
    templateUrl: './order-tool-bar.component.html',
    styleUrls: ['./order-tool-bar.component.scss']
})
export class OrderToolBarComponent implements OnInit {
    selectedDate: Date;

    constructor(private toolBarService: OrderToolBarService) {
    }

    ngOnInit(): void {
    }

    dateChanged($event: Date) {
        this.toolBarService.emit({type: 'date', payload: $event});
    }

    exportToExcel() {
        this.toolBarService.emit({type: 'export', payload: 'excel'});
    }

    exportToPrint() {
        this.toolBarService.emit({type: 'export', payload: 'print'});
    }
}
