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
import {OrdersComponent} from "./orders.component";
import {MealItemsResolver} from "./resolvers/meal-items.resolver";
import {OrdersResolver} from "./resolvers/orders-resolver.service";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatNativeDateModule} from "@angular/material/core";
import {OrderToolBarComponent} from './order-tool-bar.component';
import {MatGridListModule} from "@angular/material/grid-list";
import {MatListModule} from "@angular/material/list";
import {ConfirmDialogComponent} from './confirm-dialog.component';
import {MatMenuModule} from "@angular/material/menu";


@NgModule({
    declarations: [ListPageComponent, OrdersComponent, NewOrderComponent, OrderToolBarComponent, ConfirmDialogComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
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
                        component: OrderToolBarComponent,
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
        MatDatepickerModule,
        MatGridListModule,
        MatListModule,
        MatMenuModule
    ]
})
export class OrderModule {
}
