import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {MealType, Order} from "../../../models/order";
import {map} from "rxjs/operators";
import {TodaySelection} from "../../../models/today-selection";
import {mapOrderItem} from "../utilities";

@Injectable({providedIn: 'root'})
export class OrdersResolver implements Resolve<TodaySelection[]> {
  constructor(private httpClient: HttpClient) {
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<TodaySelection[]> | Promise<TodaySelection[]> | TodaySelection[] {
    return this.httpClient.get<Order[]>("orders", {observe: "body"}).pipe(map(orders => orders.map<TodaySelection>(mapOrderItem)));
  }
}
