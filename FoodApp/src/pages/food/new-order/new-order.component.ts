import {Component, Inject} from '@angular/core';
import {FormBuilder, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-new-order',
  templateUrl: './new-order.component.html',
  styleUrls: ['./new-order.component.scss']
})
export class NewOrderComponent {
  newOrder = this.fb.group({
    fullName: [null, Validators.required],
    mains: [null, Validators.required],
    sideOrders: [null, Validators.required],
    salad: [null, Validators.required],
    soup: [null, Validators.required],
  });
  public dataSource: { [key: string]: string[] };

  constructor(private fb: FormBuilder, private dialogRef: MatDialogRef<NewOrderComponent>, @Inject(MAT_DIALOG_DATA) data) {
    this.dataSource = data;
  }

  onSubmit() {
    if (this.newOrder.valid)
      this.dialogRef.close(this.newOrder.value)
  }

  closeDialog() {
    this.dialogRef.close()
  }
}
