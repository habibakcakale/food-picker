import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Data, Router} from "@angular/router";
import {UserService} from "../../../services/user.service";

const AUTH_TOKEN = "AuthToken";

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
    private url: string;

    constructor(private route: ActivatedRoute, userService: UserService, private router: Router) {
        window["getToken"] = (token) => {
            localStorage.setItem(AUTH_TOKEN, token);
            return userService.init().then(u => {
                return this.router.navigate(['/'])
            }).catch(c => console.log(c))
        }
    }

    ngOnInit(): void {
        this.route.data.subscribe(data => {
            this.url = data.url;
        })
    }

    login() {
        let params = `scrollbars=no,resizable=no,status=no,location=no,toolbar=no,menubar=no,width=450,height=600`;
        window.open(this.url, "hello", params);
    }
}
