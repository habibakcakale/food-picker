import {AfterViewInit, Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTable, MatTableDataSource} from '@angular/material/table';
import {MatDialog} from "@angular/material/dialog";
import {NewOrderComponent} from "./new-order/new-order.component";
import {ActivatedRoute} from "@angular/router";
import {FoodItem, FoodType} from "./list-page.component";
import {HttpClient} from "@angular/common/http";

export interface PersonFoodSelection {
  id: number;
  fullName: string;
  mains: string;
  sideOrders: string;
  salad: string;
  soup: string;
}

@Component({
  selector: 'app-today-selection',
  templateUrl: './today-selection.component.html',
  styleUrls: ['./today-selection.component.scss']
})
export class TodaySelectionComponent implements AfterViewInit, OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTable) table: MatTable<PersonFoodSelection>;
  dataSource: MatTableDataSource<PersonFoodSelection>;

  displayedColumns = ['fullName', 'mains', 'sideOrders', 'salad', 'soup', 'actions'];
  private foodGroups: { [type: string]: string[] };

  constructor(private elementRef: ElementRef<HTMLElement>, public dialog: MatDialog, private activatedRoute: ActivatedRoute, private httpClient: HttpClient) {
  }

  ngOnInit() {
    this.foodGroups = this.groupByTypes(this.activatedRoute.snapshot.data.foodItems || []);
    const todaySelections: PersonFoodSelection[] = this.activatedRoute.snapshot.data.todaySelections || [];
    this.dataSource = new MatTableDataSource<PersonFoodSelection>(todaySelections);
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.table.dataSource = this.dataSource;
  }

  openNewOrder() {
    const dialogRef = this.dialog.open(NewOrderComponent, {
      data: {
        mains: this.foodGroups[FoodType.mains],
        sideOrders: this.foodGroups[FoodType.sideOrders],
        salad: this.foodGroups[FoodType.salad],
        soup: this.foodGroups[FoodType.soup]
      }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.httpClient.post<PersonFoodSelection>("/todaysselection", result).toPromise().then(res => {
          this.dataSource.data.push(res);
          this.dataSource.filter = '';
        })
      }
    });
  }

  groupByTypes(items: FoodItem[]) {
    return items.reduce((prev, curr) => {
      if (!prev[curr.type]) {
        prev[curr.type] = []
      }
      prev[curr.type].push(curr.name);
      return prev;
    }, {} as { [key: string]: string[] })
  }

  removeOrder(row: PersonFoodSelection) {
    if (row.id)
      this.httpClient.delete<PersonFoodSelection>(`/todaysselection/${row.id}`).toPromise().finally(console.log)
    const index = this.dataSource.data.indexOf(row);
    this.dataSource.data.splice(index, 1);
    this.dataSource.filter = '';
  }

  print() {
    const table = this.elementRef.nativeElement.querySelector("table");
    const cloned = <HTMLTableElement>table.cloneNode(true);
    cloned.style.width = "100%";
    cloned.querySelectorAll(".delete-col").forEach(r => r.remove());
    const newTab = open("about:blank");
    newTab.document.body.appendChild(cloned);
    newTab.print();
  }
}
