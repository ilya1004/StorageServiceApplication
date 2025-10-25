import {ChangeDetectionStrategy, Component, inject, ViewChild} from '@angular/core';
import {DetailsList} from '../../features/details/details-list/details-list';
import {MatButton} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {CreateDetailPopup} from '../../features/details/create-detail-popup/create-detail-popup';

@Component({
  selector: 'app-details-page',
  imports: [
    DetailsList,
    MatButton
  ],
  templateUrl: './details-page.html',
  styleUrl: './details-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DetailsPage {
  private readonly dialog = inject(MatDialog)

  @ViewChild(DetailsList) detailsList!: DetailsList

  openCreateDetailPopup(): void {
    const dialogRef = this.dialog.open(CreateDetailPopup, {
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.detailsList.loadDetails();
      }
    });
  }
}
