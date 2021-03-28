import {NgModule} from '@angular/core';
import {Routes, RouterModule, Resolve} from '@angular/router';
import {NavigationComponent} from "./layouts/navigation.component";
import {CommonModule} from "@angular/common";
import {LoginComponent} from "./pages/auth/login/login.component";
import {AuthGuard} from "./guards/auth.guard";
import {GoogleLoginUrlResolver} from "./pages/auth/login/google-login-url.resolver";


const routes: Routes = [
    {path: "", pathMatch: "full", redirectTo: "today"},
    {
        path: "",
        component: NavigationComponent,
        canActivate: [AuthGuard],
        loadChildren: () => import("./pages/order/order.module").then(m => m.OrderModule)
    },
    {
        path: "login",
        component: LoginComponent,
        resolve: {url: GoogleLoginUrlResolver}
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

