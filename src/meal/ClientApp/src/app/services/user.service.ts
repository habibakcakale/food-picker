import {Injectable, InjectionToken} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {User} from "../models/user";

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private _user: User;
    public get user(): User {
        return this._user
    };

    constructor(private client: HttpClient) {
    }

    init() {
        return new Promise((resolve, reject) => {
            const headers = new HttpHeaders({
                "Authorization": `Bearer ${localStorage.getItem("AuthToken")}`
            });
            this.client.get<User>("account", {
                headers: headers
            }).toPromise().then((resp) => {
                this._user = resp
                resolve(resp);
            }).catch(() => {
                this._user = null;
                resolve(null);
            })
        })
    }
}
