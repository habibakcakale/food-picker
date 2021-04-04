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
import {map} from "rxjs/operators";
import {Subscription} from "rxjs";
import {ConfirmDialogComponent} from "./confirm-dialog.component";
import {OrderToolBarService} from "./order-tool-bar.service";
import {UntilDestroy, untilDestroyed} from "@ngneat/until-destroy";
import * as Url from "url";

@Component({
    selector: 'app-today-selection',
    templateUrl: './orders.component.html',
    styleUrls: ['./orders.component.scss']
})
@UntilDestroy()
export class OrdersComponent implements AfterViewInit, OnInit, OnDestroy {
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;
    dataSource: MatTableDataSource<TodaySelection>;
    displayedColumns = ['name', 'mains', 'sideOrders', 'salad', 'actions'];
    user: User;
    private foodGroups: { [type: string]: Meal[] };
    private $dateSub: Subscription;
    formatter = new Intl.DateTimeFormat("en-US", {
        month: "numeric",
        year: "numeric",
        day: "numeric",
        timeZone: "Europe/Istanbul"
    });

    constructor(private elementRef: ElementRef<HTMLElement>,
                public dialog: MatDialog,
                private activatedRoute: ActivatedRoute,
                private httpClient: HttpClient,
                toolBarService: OrderToolBarService,
                userService: UserService) {
        this.user = userService.user;
        this.$dateSub = toolBarService.state$.pipe(untilDestroyed(this)).subscribe(({type, payload}) => {
            switch (type) {
                case 'date':
                    this.getOrders(payload);
                    break;
                case 'export':
                    if (payload == 'excel') {
                        this.downloadToCsv();
                    } else if (payload == 'print') {
                        this.print();
                    }
                    break;
            }
        })
    }

    getOrders(date: Date) {
        return this.httpClient.get<Order[]>(`orders?dateTime=${this.formatter.format(date)}`, {observe: "body"}).pipe(map(orders => orders.map<TodaySelection>(mapOrderItem))).subscribe(orders => {
            this.dataSource.data = orders;
            this.dataSource.filter = '';
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
                const orderItems = [...result.mains, ...result.sideOrders, ...result.salads];
                this.httpClient.post<Order>("orders", {orderItems, date: result.date}).toPromise().then(res => {
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
            data: {fullName: row.user?.name}
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

    downloadToCsv() {
        const data = this.dataSource.data.map(item => `${item.user?.name},${item.mains},${item.sideOrders},${item.salad}`);
        data.unshift('Isim,Ana Yemek,Ara Yemek,Salata');
        const blob = new Blob([data.join('\n')], {type: 'text/csv;charset=utf-8;'});
        const url = URL.createObjectURL(blob);
        window.open(url)
    }

    print() {
        const rows = this.dataSource.data.map(item => `<tr><td style="font-weight: 700;">${item.user?.name}</td><td>${item.mains}</td><td>${item.sideOrders}</td><td>${item.salad}</td></tr>`)
        const table = `\n<link rel="stylesheet" href="https://getbootstrap.com/docs/4.0/dist/css/bootstrap.min.css">\n<table class="table table-stripped">\n<thead>\n    <tr>\n        <th>Isim</th>\n        <th>Ana Yemek</th>\n        <th>Ara Yemek</th>\n        <th>Salata</th>\n    </tr>\n  </thead>\n  <tbody>\n  ${rows.join("")}\n  </tbody>\n  </table>`;
        const newTab = open("about:blank");
        newTab.document.body.innerHTML = table;
    }

    ngOnDestroy(): void {
        this.$dateSub.unsubscribe();
    }
}
