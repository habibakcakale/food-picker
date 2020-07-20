import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {PersonFoodSelection} from "./today-selection.component";

@Injectable({providedIn: 'root'})
export class TodaySelectionsResolver implements Resolve<PersonFoodSelection[]> {
  constructor(private httpClient: HttpClient) {
  }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<PersonFoodSelection[]> | Promise<PersonFoodSelection[]> | PersonFoodSelection[] {
    return this.httpClient.get<PersonFoodSelection[]>("todaysselection", {observe: "body"});
  }
}
