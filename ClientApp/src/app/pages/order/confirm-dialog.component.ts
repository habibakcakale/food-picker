import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
    selector: 'app-confirm-dialog',
    templateUrl: './confirm-dialog.component.html',
    styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {
    fullName: string

    constructor(private dialogRef: MatDialogRef<ConfirmDialogComponent>,
                @Inject(MAT_DIALOG_DATA) data) {
        this.fullName = data && data.fullName;
    }

    ngOnInit(): void {
    }

    closeDialog() {
        this.dialogRef.close();
    }

    delete() {
        this.dialogRef.close(true);
    }
}
