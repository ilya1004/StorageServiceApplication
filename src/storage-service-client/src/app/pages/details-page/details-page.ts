import { ChangeDetectionStrategy, Component } from '@angular/core';
import {DetailsList} from '../../features/details/details-list/details-list';

@Component({
  selector: 'app-details-page',
  imports: [
    DetailsList
  ],
  templateUrl: './details-page.html',
  styleUrl: './details-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DetailsPage {

}
