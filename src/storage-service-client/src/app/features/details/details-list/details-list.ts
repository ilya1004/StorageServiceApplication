import {ChangeDetectionStrategy, Component, inject, OnInit, signal} from '@angular/core';
import {Detail} from '../../../core/models/details/detail';
import {
  MatCell, MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef,
  MatRow, MatRowDef,
  MatTable,
  MatTableDataSource
} from '@angular/material/table';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {DetailsService} from '../../../core/services/details-service/details-service';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {DeleteDetailDialog} from '../delete-detail-dialog/delete-detail-dialog';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-details-list',
  imports: [
    MatTable,
    MatColumnDef,
    MatHeaderCell,
    MatCell,
    MatHeaderRow,
    MatRow,
    MatPaginator,
    MatHeaderCellDef,
    MatRowDef,
    MatHeaderRowDef,
    MatCellDef,
    MatIcon,
    MatButton,
    DatePipe
  ],
  templateUrl: './details-list.html',
  styleUrl: './details-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DetailsList implements OnInit {
  private readonly detailsService = inject(DetailsService);
  private readonly dialog = inject(MatDialog);

  readonly displayedColumns: string[] = ['id', 'nomenclatureCode', 'name', 'count', 'storekeeper', 'createdAtDate', 'actions'];

  pageIndex = signal<number>(0);
  pageSize = signal<number>(5);
  totalCount = signal<number>(0);
  details =
    signal<MatTableDataSource<Detail>>(new MatTableDataSource<Detail>());

  ngOnInit(): void {
    this.loadDetails();
  }

  loadDetails(): void {
    this.detailsService.getPaginated({pageNo: this.pageIndex() + 1, pageSize: this.pageSize() }).subscribe({
      next: (paginatedResult) => {
        this.details.set(new MatTableDataSource(paginatedResult.items));
        this.pageIndex.set(paginatedResult.pageNo - 1);
        this.pageSize.set(paginatedResult.pageSize);
        this.totalCount.set(paginatedResult.totalCount);
      },
      error: (err) => {
        console.error(err)
      }
    })
  }

  onPageChange(pageEvent: PageEvent) {
    this.pageSize.set(pageEvent.pageSize);
    this.pageIndex.set(pageEvent.pageIndex);
    this.totalCount.set(pageEvent.length);
    this.loadDetails();
  }

  onDeleteItemClick(id: number): void {
    const dialogRef = this.dialog.open(DeleteDetailDialog);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.detailsService.delete(id).subscribe({
          next: () => {
            this.loadDetails();
          },
          error: (err) => {
            console.error(err);
          }
        })
      }
    })
  }
}
