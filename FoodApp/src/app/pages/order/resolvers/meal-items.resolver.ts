import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {Meal} from "../../../models/order";

@Injectable({providedIn: 'root'})
export class MealItemsResolver implements Resolve<Meal[]> {
  constructor(private httpClient: HttpClient) {
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Meal[]> | Promise<Meal[]> | Meal[] {
    return this.httpClient.get<Meal[]>("meals", {observe: "body"});
  }
}
