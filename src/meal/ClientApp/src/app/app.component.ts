import {ApplicationRef, Component} from '@angular/core';
import {SwUpdate} from "@angular/service-worker";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    title = 'ClientApp';

    constructor(appRef: ApplicationRef, update: SwUpdate) {
        let interval = setInterval(() => {
            console.log(`update enabled ${update.isEnabled}`);
            if (update.isEnabled) {
                clearInterval(interval);
                return this.updateApp(update);
            }
        }, 1000);
    }

    updateApp(update: SwUpdate) {
        update.available.subscribe(e => {
            console.log(`update available`, e);
            update.activateUpdate().then(e => location.reload())
        });
        return update.checkForUpdate();
    }
}
