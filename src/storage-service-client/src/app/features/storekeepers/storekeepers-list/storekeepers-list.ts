import {AfterViewInit, ChangeDetectionStrategy, Component, inject, OnInit, ViewChild} from '@angular/core';
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
import {MatPaginator} from '@angular/material/paginator';
import {StorekeepersService} from '../../../core/services/storekeepers-service/storekeepers-service';
import {MatDialog} from '@angular/material/dialog';
import {Storekeeper} from '../../../core/models/storekeepers/storekeeper';
import {DeleteStorekeeperDialog} from '../delete-storekeeper-dialog/delete-storekeeper-dialog';

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
export class StorekeepersList implements OnInit, AfterViewInit {
  private readonly storekeepersService = inject(StorekeepersService);
  private readonly dialog = inject(MatDialog);

  displayedColumns: string[] = ['id', 'fullName', 'actions'];

  storekeepers = new MatTableDataSource<Storekeeper>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.loadStorekeepers();
  }

  loadStorekeepers(): void {
    this.storekeepersService.getAll().subscribe({
      next: (storekeepers) => {
        this.storekeepers = new MatTableDataSource(storekeepers);
      },
      error: (err) => {
        console.error(err)
      }
    })
  }

  ngAfterViewInit(): void {
    this.storekeepers.paginator = this.paginator;
  }

  onDeleteItemClick(id: number): void {
    const dialogRef = this.dialog.open(DeleteStorekeeperDialog);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.storekeepersService.delete(id).subscribe({
          next: () => {
            this.loadStorekeepers();
          },
          error: (err) => {
            console.error(err);
          }
        })
      }
    })
  }
}
