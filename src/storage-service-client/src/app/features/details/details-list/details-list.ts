import {AfterViewInit, ChangeDetectionStrategy, Component, inject, OnInit, ViewChild} from '@angular/core';
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
import {MatPaginator} from '@angular/material/paginator';
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
export class DetailsList implements OnInit, AfterViewInit {
  private readonly detailsService = inject(DetailsService);
  private readonly dialog = inject(MatDialog);

  displayedColumns: string[] = ['id', 'nomenclatureCode', 'name', 'count', 'storekeeper', 'createdAtDate', 'actions'];

  details = new MatTableDataSource<Detail>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.loadDetails();
  }

  loadDetails(): void {
    this.detailsService.getAll().subscribe({
      next: (details) => {
        this.details = new MatTableDataSource(details);
      },
      error: (err) => {
        console.error(err)
      }
    })
  }

  ngAfterViewInit(): void {
    this.details.paginator = this.paginator;
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
