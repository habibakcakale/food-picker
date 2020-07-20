import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {FoodItem} from "./list-page.component";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({providedIn: 'root'})
export class FoodItemsResolver implements Resolve<FoodItem[]> {
  constructor(private httpClient: HttpClient) {
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<FoodItem[]> | Promise<FoodItem[]> | FoodItem[] {
    return this.httpClient.get<FoodItem[]>("foods", {observe: "body"});
  }
}
