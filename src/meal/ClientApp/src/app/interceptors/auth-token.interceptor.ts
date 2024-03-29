import {Injectable} from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor, HttpErrorResponse
} from '@angular/common/http';
import {Observable, of} from 'rxjs';
import {catchError} from "rxjs/operators";
import {UserService} from "../services/user.service";
import {Router} from "@angular/router";

const AUTH_TOKEN = "AuthToken";

@Injectable({
    providedIn: "root"
})
export class AuthTokenInterceptor implements HttpInterceptor {
    constructor(private userService: UserService, private router: Router) {
    }

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        const token = localStorage.getItem(AUTH_TOKEN);

        if (token && !request.headers.has("Authorization")) {
            return next.handle(request.clone({
                headers: request.headers.set("Authorization", `Bearer ${localStorage.getItem(AUTH_TOKEN)}`)
            })).pipe(catchError((err, resp) => {
                if (err instanceof HttpErrorResponse && 500 > err.status && err.status >= 400) {
                    localStorage.removeItem(AUTH_TOKEN)
                    return this.router.navigate(["login"]);
                }
                return of(err)
            }));
        }
        return next.handle(request)
    }
}
