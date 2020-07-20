import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {NavigationComponent} from "./layouts/navigation.component";
import {CommonModule} from "@angular/common";


const routes: Routes = [
  {
    path: "", pathMatch: "full", redirectTo: "food"
  },
  {
    path: "food",
    component: NavigationComponent,
    loadChildren: () => import("./../pages/food/food.module").then(m => m.FoodListModule)
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
