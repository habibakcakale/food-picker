import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from "@angular/router";
import {ListPageComponent} from "./list-page.component";
import {TodaySelectionComponent} from './today-selection.component';
import {MatTableModule} from '@angular/material/table';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatSortModule} from '@angular/material/sort';
import {MatIconModule} from "@angular/material/icon";
import {MatButtonModule} from "@angular/material/button";
import {NewOrderComponent} from './new-order/new-order.component';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatRadioModule} from '@angular/material/radio';
import {MatCardModule} from '@angular/material/card';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatDialogModule} from "@angular/material/dialog";
import {FoodItemsResolver} from "./food-items.resolver";
import {HttpClientModule} from "@angular/common/http";
import {TodaySelectionsResolver} from "./today-selections.resolver";


@NgModule({
  declarations: [ListPageComponent, TodaySelectionComponent, NewOrderComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forChild([
      {path: "", pathMatch: "full", redirectTo: "list"},
      {
        path: "list", component: ListPageComponent, resolve: {
          foodItems: FoodItemsResolver
        }
      },
      {
        path: "today", component: TodaySelectionComponent, resolve: {
          foodItems: FoodItemsResolver,
          todaySelections: TodaySelectionsResolver
        }
      }
    ]),
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatSelectModule,
    MatRadioModule,
    MatCardModule,
    MatDialogModule
  ]
})
export class FoodListModule {
}
