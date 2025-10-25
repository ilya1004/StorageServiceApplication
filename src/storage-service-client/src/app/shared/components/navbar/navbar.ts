import {ChangeDetectionStrategy, Component} from '@angular/core';
import {MatSlideToggle} from '@angular/material/slide-toggle';
import {MatToolbar} from '@angular/material/toolbar';
import {MatButton} from '@angular/material/button';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [
    MatToolbar,
    MatButton,
    RouterLink

  ],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Navbar {
  navItems = [
    { name: 'Details', route: '/details' },
    { name: 'Storekeepers', route: '/storekeepers' }
  ];
}
