import {ChangeDetectionStrategy, Component} from '@angular/core';
import {NgOptimizedImage} from '@angular/common';

@Component({
  selector: 'app-footer',
  imports: [
    NgOptimizedImage
  ],
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Footer {

}
