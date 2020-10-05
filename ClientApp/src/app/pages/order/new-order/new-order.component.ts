import {Component, Inject} from '@angular/core';
import {AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {User} from "../../../models/user";
import {UserService} from "../../../services/user.service";
import {Meal} from "../../../models/order";

@Component({
    selector: 'app-new-order',
    templateUrl: './new-order.component.html',
    styleUrls: ['./new-order.component.scss']
})
export class NewOrderComponent {
    user: User;
    maxDate = new Date()

    newOrder: FormGroup = this.fb.group({
        mains: this.fb.array([new FormControl(null, Validators.required)], Validators.required),
        sideOrders: this.fb.array([new FormControl()]),
        salads: this.fb.array([new FormControl()]),
        date: [new Date().toISOString(), Validators.required]
    })
    public dataSource: { [key: string]: Meal[] };

    constructor(
        userService: UserService,
        private fb: FormBuilder,
        private dialogRef: MatDialogRef<NewOrderComponent>,
        @Inject(MAT_DIALOG_DATA) data) {
        this.user = userService.user;
        this.dataSource = data;
    }

    onSubmit() {
        if (this.newOrder.valid)
            this.dialogRef.close(this.newOrder.value)
    }

    closeDialog() {
        this.dialogRef.close()
    }

    hasError(control: string | FormControl, error: string) {
        if (control instanceof FormControl)
            return control.hasError(error);
        return this.newOrder && this.newOrder.controls[control].hasError(error);
    }

    getControls(control: string): FormControl[] {
        return (this.newOrder.get(control) as FormArray).controls as FormControl[];
    }

    addControl($event: MouseEvent, control: string) {
        $event.preventDefault();
        $event.stopImmediatePropagation();
        (this.newOrder.get(control) as FormArray).push(new FormControl());
    }

    removeControl($event: MouseEvent, control: string, toRemove: FormControl) {
        const form = (this.newOrder.get(control) as FormArray);
        if (form.length == 1)
            return
        form.removeAt(form.controls.indexOf(toRemove))
    }
}
