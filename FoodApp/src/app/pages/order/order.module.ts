import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from "@angular/router";
import {ListPageComponent} from "./list-page.component";
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
import {HttpClientModule} from "@angular/common/http";
import {OrdersComponent} from "./orders.component";
import {MealItemsResolver} from "./resolvers/meal-items.resolver";
import {OrdersResolver} from "./resolvers/orders-resolver.service";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatNativeDateModule} from "@angular/material/core";
import {OrderDatePickerComponent} from './order-date-picker.component';


@NgModule({
  declarations: [ListPageComponent, OrdersComponent, NewOrderComponent, OrderDatePickerComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forChild([
      {path: "", pathMatch: "full", redirectTo: "today"},
      {
        path: "list", component: ListPageComponent, resolve: {
          foodItems: MealItemsResolver
        }
      },
      {
        path: "today", children: [{
          path: '',
          component: OrdersComponent, resolve: {
            foodItems: MealItemsResolver,
            orders: OrdersResolver
          }
        },
          {
            path: '',
            component: OrderDatePickerComponent,
            outlet: 'toolbar'
          }],
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
    MatDialogModule,
    MatNativeDateModule,
    MatDatepickerModule
  ]
})
export class OrderModule {
}
