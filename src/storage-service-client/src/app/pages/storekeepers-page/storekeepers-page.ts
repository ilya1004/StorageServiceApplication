import {ChangeDetectionStrategy, Component, inject, ViewChild} from '@angular/core';
import {StorekeepersList} from '../../features/storekeepers/storekeepers-list/storekeepers-list';
import {MatButton} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {CreateStorekeeperPopup} from '../../features/storekeepers/create-storekeeper-popup/create-storekeeper-popup';

@Component({
  selector: 'app-storekeepers-page',
  imports: [
    MatButton,
    StorekeepersList
  ],
  templateUrl: './storekeepers-page.html',
  styleUrl: './storekeepers-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StorekeepersPage {
  private readonly dialog = inject(MatDialog)

  @ViewChild(StorekeepersList) storekeepersList!: StorekeepersList

  openCreateStorekeeperPopup(): void {
    const dialogRef = this.dialog.open(CreateStorekeeperPopup, {
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.storekeepersList.loadStorekeepers();
      }
    });
  }
}
