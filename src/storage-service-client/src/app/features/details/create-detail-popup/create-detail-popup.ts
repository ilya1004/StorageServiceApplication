import {ChangeDetectionStrategy, Component, inject, OnInit, signal} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Storekeeper} from '../../../core/models/storekeepers/storekeeper';
import {MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {DetailsService} from '../../../core/services/details-service/details-service';
import {CreateDetailDto} from '../../../core/models/details/createDetailDto';
import {StorekeepersService} from '../../../core/services/storekeepers-service/storekeepers-service';
import {ICreateDetailForm} from './create-detail.form';
import {MatError, MatFormField, MatSuffix} from '@angular/material/form-field';
import {MatLabel} from '@angular/material/form-field';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatInput} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatButton} from '@angular/material/button';
import {MatSnackBar} from '@angular/material/snack-bar';
import {HttpErrorResponse} from '@angular/common/http';

@Component({
  selector: 'app-create-detail-popup',
  imports: [
    MatFormField,
    MatLabel,
    MatDatepickerInput,
    MatInput,
    MatDatepickerToggle,
    MatDatepicker,
    MatDialogTitle,
    MatDialogContent,
    ReactiveFormsModule,
    MatSelect,
    MatOption,
    MatDialogActions,
    MatButton,
    MatSuffix,
    MatError
  ],
  templateUrl: './create-detail-popup.html',
  styleUrl: './create-detail-popup.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateDetailPopup implements OnInit {
  private readonly dialogRef: MatDialogRef<CreateDetailPopup> = inject(MatDialogRef<CreateDetailPopup>)
  private readonly detailsService = inject(DetailsService)
  private readonly storekeepersService = inject(StorekeepersService)
  private readonly snackBar = inject(MatSnackBar);

  storekeepers = signal<Storekeeper[]>([]);

  detailForm = new FormGroup<ICreateDetailForm>({
    nomenclatureCode: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2), Validators.maxLength(200)],
    }),
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2), Validators.maxLength(200)],
    }),
    count: new FormControl(1, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1)]
    }),
    storekeeperId: new FormControl(null, {
      nonNullable: false,
      validators: [Validators.required],
    }),
    createdAtDate: new FormControl(new Date(), {
      nonNullable: true,
      validators: [Validators.required]
    }),
  });

  ngOnInit(): void {
    this.storekeepersService.getAll("lookup").subscribe({
      next: (storekeepers) => {
        this.storekeepers.set(storekeepers);
      },
      error: (err: HttpErrorResponse) => {
        this.showError(err.error.detail || 'An error occurred while retrieving storekeepers');
      }
    });
  }

  onSubmit(): void {
    if (this.detailForm.valid) {
      const formValue = this.detailForm.value;
      const createDetailDto: CreateDetailDto = {
        nomenclatureCode: formValue.nomenclatureCode!,
        name: formValue.name!,
        count: formValue.count!,
        storekeeperId: formValue.storekeeperId!,
        createdAtDate: formValue.createdAtDate?.toISOString()!,
      };

      this.detailsService.create(createDetailDto).subscribe({
        next: (detail) => {
          this.dialogRef.close(detail);
        },
        error: (err: HttpErrorResponse) => {
          this.showError(err.error.detail || 'An error occurred while creating the detail');
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Dismiss', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['error-snackbar']
    });
  }
}
