import { Component } from '@angular/core';
import {MatSlideToggle} from '@angular/material/slide-toggle';

@Component({
  selector: 'app-navbar',
  imports: [
    MatSlideToggle
  ],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {

}
