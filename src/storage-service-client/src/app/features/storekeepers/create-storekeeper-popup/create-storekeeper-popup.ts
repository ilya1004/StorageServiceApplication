import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { StorekeepersService } from '../../../core/services/storekeepers-service/storekeepers-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ICreateStorekeeperForm } from './create-storekeeper.form';
import { CreateStorekeeperDto } from '../../../core/models/storekeepers/createStorekeeperDto';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField } from '@angular/material/form-field';
import { MatInput, MatLabel } from '@angular/material/input';

@Component({
  selector: 'app-create-storekeeper-popup',
  imports: [
    MatButton,
    MatDialogActions,
    MatDialogContent,
    MatDialogTitle,
    MatError,
    MatFormField,
    MatInput,
    MatLabel,
    ReactiveFormsModule
  ],
  templateUrl: './create-storekeeper-popup.html',
  styleUrl: './create-storekeeper-popup.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateStorekeeperPopup {
  private readonly dialogRef: MatDialogRef<CreateStorekeeperPopup> = inject(MatDialogRef<CreateStorekeeperPopup>)
  private readonly storekeepersService = inject(StorekeepersService)

  storekeeperForm = new FormGroup<ICreateStorekeeperForm>({
    fullName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2), Validators.maxLength(200)],
    }),
  });

  onSubmit(): void {
    if (this.storekeeperForm.valid) {
      const formValue = this.storekeeperForm.value;
      const createStorekeeperDto: CreateStorekeeperDto = {
        fullName: formValue.fullName!,
      };

      this.storekeepersService.create(createStorekeeperDto).subscribe({
        next: (storekeeper) => {
          this.dialogRef.close(storekeeper);
        },
        error: (err) => {
          console.error(err);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
