import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {NavigationComponent} from "./layouts/navigation.component";
import {CommonModule} from "@angular/common";


const routes: Routes = [
  {
    path: "", pathMatch: "full", redirectTo: "today"
  },
  {
    path: "",
    component: NavigationComponent,
    loadChildren: () => import("./pages/order/order.module").then(m => m.OrderModule)
  }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
