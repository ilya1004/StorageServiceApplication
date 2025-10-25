import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from '@angular/material/dialog';
import {MatButton} from '@angular/material/button';

@Component({
  selector: 'app-delete-detail-dialog',
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatButton
  ],
  templateUrl: './delete-detail-dialog.html',
  styleUrl: './delete-detail-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeleteDetailDialog {
  private readonly dialogRef: MatDialogRef<DeleteDetailDialog> = inject(MatDialogRef<DeleteDetailDialog>)

  onCancel(): void {
    this.dialogRef.close();
  }
}
