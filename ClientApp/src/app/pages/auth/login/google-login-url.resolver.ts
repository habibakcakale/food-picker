import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {map} from "rxjs/operators";

@Injectable({
    providedIn: "root"
})
export class GoogleLoginUrlResolver implements Resolve<string> {
    constructor(private httpClient: HttpClient) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<string> | Promise<string> | string {
        return this.httpClient.get<{ url: string }>("/Account/BuildChallengeUrl").pipe(map(data => data.url));
    }
}