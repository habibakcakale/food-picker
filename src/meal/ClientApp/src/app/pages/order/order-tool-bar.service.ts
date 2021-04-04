import {Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class OrderToolBarService {
    public readonly state$ = new BehaviorSubject<{ type?: string, payload?: any }>({})

    emit(data: { type: string, payload: any }) {
        this.state$.next(data);
    }
}
