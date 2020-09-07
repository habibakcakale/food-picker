import {Injectable, InjectionToken} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {User} from "../models/user";

export const USER_TOKEN = new InjectionToken("APP_USER");

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
    return this.client.get<User>("account/userinfo").toPromise().then((resp) => {
      this._user = resp
    })
  }
}
