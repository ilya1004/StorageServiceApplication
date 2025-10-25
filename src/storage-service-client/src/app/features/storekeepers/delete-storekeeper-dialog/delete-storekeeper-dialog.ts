import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {MatButton} from "@angular/material/button";
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from "@angular/material/dialog";

@Component({
  selector: 'app-delete-storekeeper-dialog',
  imports: [
    MatButton,
    MatDialogActions,
    MatDialogClose,
    MatDialogContent,
    MatDialogTitle
  ],
  templateUrl: './delete-storekeeper-dialog.html',
  styleUrl: './delete-storekeeper-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeleteStorekeeperDialog {
  private readonly dialogRef: MatDialogRef<DeleteStorekeeperDialog> = inject(MatDialogRef<DeleteStorekeeperDialog>)

  onCancel(): void {
    this.dialogRef.close();
  }
}
