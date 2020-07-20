import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTable, MatTableDataSource} from '@angular/material/table';
import {ActivatedRoute} from "@angular/router";
import {HttpClient} from "@angular/common/http";

export enum FoodType {
  mains = "Ana Yemek",
  sideOrders = "Ara Yemek",
  salad = "Salata",
  soup = "Corba"
}

export interface FoodItem {
  id: number,
  name: string;
  type: FoodType;
}

@Component({
  selector: 'app-list-page',
  templateUrl: './list-page.component.html',
  styleUrls: ['./list-page.component.scss']
})
export class ListPageComponent implements AfterViewInit, OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTable) table: MatTable<FoodItem>;
  dataSource: MatTableDataSource<FoodItem>;

  foodTypes = [
    FoodType.mains,
    FoodType.sideOrders,
    FoodType.salad,
    FoodType.soup
  ]

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['name', 'type', 'actions'];

  constructor(private activatedRoute: ActivatedRoute, public client: HttpClient) {
  }

  ngOnInit() {
    const items: FoodItem[] = this.activatedRoute.snapshot.data.foodItems || [];
    this.dataSource = new MatTableDataSource(items);
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.table.dataSource = this.dataSource;
  }

  addNewFood() {
    this.dataSource.data.push({id: 0, name: '', type: FoodType.mains});
    this.dataSource.filter = '';
  }

  saveFoods() {
    const newItems = this.dataSource.data.filter(i => !i.id)
    this.client.post<FoodItem[]>('/foods', newItems).toPromise().then(res => {
      res.forEach((item, index) => newItems[index].id = item.id);
    })
  }

  removeFood(row: FoodItem) {
    if (row.id)
      this.client.delete(`/foods/${row.id}`).toPromise().finally(console.log);
    const index = this.dataSource.data.indexOf(row);
    this.dataSource.data.splice(index, 1);
    this.dataSource.filter = '';
  }
}
