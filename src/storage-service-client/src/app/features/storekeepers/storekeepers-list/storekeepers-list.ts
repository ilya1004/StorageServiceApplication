import {ChangeDetectionStrategy, Component, inject, OnInit, signal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow, MatRowDef, MatTable, MatTableDataSource
} from '@angular/material/table';
import {MatIcon} from '@angular/material/icon';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {StorekeepersService} from '../../../core/services/storekeepers-service/storekeepers-service';
import {MatDialog} from '@angular/material/dialog';
import {Storekeeper} from '../../../core/models/storekeepers/storekeeper';
import {DeleteStorekeeperDialog} from '../delete-storekeeper-dialog/delete-storekeeper-dialog';
import {HttpErrorResponse} from '@angular/common/http';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-storekeepers-list',
  imports: [
    MatButton,
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderRow,
    MatHeaderRowDef,
    MatIcon,
    MatPaginator,
    MatRow,
    MatRowDef,
    MatTable,
    MatHeaderCellDef
  ],
  templateUrl: './storekeepers-list.html',
  styleUrl: './storekeepers-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StorekeepersList implements OnInit {
  private readonly storekeepersService = inject(StorekeepersService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly displayedColumns: string[] = ['id', 'fullName', 'detailsCount', 'actions'];

  pageIndex = signal<number>(0);
  pageSize = signal<number>(5);
  totalCount = signal<number>(0);
  storekeepers =
    signal<MatTableDataSource<Storekeeper>>(new MatTableDataSource<Storekeeper>());

  ngOnInit(): void {
    this.loadStorekeepers();
  }

  loadStorekeepers(): void {
    this.storekeepersService.getPaginated({pageNo: this.pageIndex() + 1, pageSize: this.pageSize() })
      .subscribe({
        next: (paginatedResult) => {
          this.storekeepers.set(new MatTableDataSource(paginatedResult.items));
          this.pageIndex.set(paginatedResult.pageNo - 1);
          this.pageSize.set(paginatedResult.pageSize);
          this.totalCount.set(paginatedResult.totalCount);
        },
        error: (err: HttpErrorResponse) => {
          this.showError(err.error.detail || 'An error occurred while retrieving storekeepers');
        }
      })
  }

  onPageChange(pageEvent: PageEvent) {
    this.pageSize.set(pageEvent.pageSize);
    this.pageIndex.set(pageEvent.pageIndex);
    this.totalCount.set(pageEvent.length);
    this.loadStorekeepers();
  }

  onDeleteItemClick(id: number): void {
    const dialogRef = this.dialog.open(DeleteStorekeeperDialog);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.storekeepersService.delete(id).subscribe({
          next: () => {
            this.loadStorekeepers();
          },
          error: (err: HttpErrorResponse) => {
            this.showError(err.error.detail || 'An error occurred while deleting the storekeeper');
          }
        })
      }
    })
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
