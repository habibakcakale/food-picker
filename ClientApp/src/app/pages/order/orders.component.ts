import {AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTable, MatTableDataSource} from '@angular/material/table';
import {MatDialog} from "@angular/material/dialog";
import {NewOrderComponent} from "./new-order/new-order.component";
import {ActivatedRoute} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {Meal, MealType, Order} from "../../models/order";
import {TodaySelection} from "../../models/today-selection";
import {mapOrderItem} from "./utilities";
import {UserService} from "../../services/user.service";
import {User} from "../../models/user";
import {OrderDateService} from "./order-date.service";
import {map} from "rxjs/operators";
import {Subscription} from "rxjs";
import {ConfirmDialogComponent} from "./confirm-dialog.component";

@Component({
    selector: 'app-today-selection',
    templateUrl: './orders.component.html',
    styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements AfterViewInit, OnInit, OnDestroy {
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;
    @ViewChild(MatTable) table: MatTable<TodaySelection>;
    dataSource: MatTableDataSource<TodaySelection>;
    displayedColumns = ['fullName', 'mains', 'sideOrders', 'salad', 'actions'];
    private foodGroups: { [type: string]: Meal[] };
    private user: User;
    private $dateSub: Subscription;

    constructor(private elementRef: ElementRef<HTMLElement>,
                public dialog: MatDialog,
                private activatedRoute: ActivatedRoute,
                private httpClient: HttpClient,
                dateService: OrderDateService,
                userService: UserService) {
        this.user = userService.user;
        const formatter = new Intl.DateTimeFormat("en-US", {
            month: "numeric",
            year: "numeric",
            day: "numeric",
            timeZone: "Europe/Istanbul"
        });
        this.$dateSub = dateService.subscribe((date: Date) => {
            this.httpClient.get<Order[]>(`orders?dateTime=${formatter.format(date)}`, {observe: "body"}).pipe(map(orders => orders.map<TodaySelection>(mapOrderItem))).subscribe(orders => {
                this.dataSource.data = orders;
                this.dataSource.filter = '';
            });
        })
    }

    ngOnInit() {
        this.foodGroups = this.groupByTypes(this.activatedRoute.snapshot.data.foodItems || []);
        const orders: TodaySelection[] = this.activatedRoute.snapshot.data.orders || [];
        this.dataSource = new MatTableDataSource<TodaySelection>(orders);
    }

    ngAfterViewInit() {
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        this.table.dataSource = this.dataSource;
    }

    openNewOrder() {
        const dialogRef = this.dialog.open(NewOrderComponent, {
            width: "700px",
            data: {
                mains: this.foodGroups[MealType.Mains],
                sideOrders: this.foodGroups[MealType.SideOrders],
                salad: this.foodGroups[MealType.Salad],
            }
        });
        const subscriber = dialogRef.afterClosed().subscribe(result => {
            subscriber.unsubscribe();
            if (result) {
                const orderItems = Object.keys(result).map(key => result[key]).reduce((prev, curr) => prev.concat(curr), []);
                this.httpClient.post<Order>("orders", orderItems).toPromise().then(res => {
                    let index = this.dataSource.data.findIndex(item => item.userId == this.user.id);
                    if (index > -1)
                        this.dataSource.data.splice(index, 1);
                    this.dataSource.data.push(mapOrderItem(res));
                    this.dataSource.filter = '';
                })
            }
        });
    }

    groupByTypes(items: Meal[]) {
        return items.reduce((prev, curr) => {
            if (!prev[curr.mealType]) {
                prev[curr.mealType] = []
            }
            prev[curr.mealType].push(curr);
            return prev;
        }, {} as { [key: string]: Meal[] })
    }

    removeOrder(row: Order) {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            data: {fullName: row.fullName}
        });
        const subscriber = dialogRef.afterClosed().subscribe(result => {
            subscriber.unsubscribe();
            if (result) {
                if (row.id)
                    this.httpClient.delete<Order>(`orders/${row.id}`).toPromise().finally(console.log)
                const index = this.dataSource.data.indexOf(row);
                this.dataSource.data.splice(index, 1);
                this.dataSource.filter = '';
            }
        })
    }

    print() {
        const rows = this.dataSource.data.map(item => `<tr><td style="font-weight: 700;">${item.fullName}</td><td>${item.mains}</td><td>${item.sideOrders}</td><td>${item.salad}</td></tr>`)
        const table = `
<link rel="stylesheet" href="https://getbootstrap.com/docs/4.0/dist/css/bootstrap.min.css">
<table class="table table-stripped">
<thead>
    <tr>
        <th>Isim</th>
        <th>Ana Yemek</th>
        <th>Ara Yemek</th>
        <th>Salata</th>
    </tr>
  </thead>
  <tbody>
  ${rows.join("")}
  </tbody>
  </table>`;
        const newTab = open("about:blank");
        newTab.document.body.innerHTML = table;
    }

    ngOnDestroy(): void {
        this.$dateSub.unsubscribe();
    }
}
