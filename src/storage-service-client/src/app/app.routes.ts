import { Routes } from '@angular/router';
import {Layout} from './shared/components/layout/layout';

export const routes: Routes = [
  {
    path: '',
    component: Layout,
    children: [
      {
        path: '',
        redirectTo: 'details',
        pathMatch: 'full'
      },
      {
        path: 'details',
        loadComponent: () =>
          import("./pages/details-page/details-page").then(x => x.DetailsPage)
      },
      {
        path: 'storekeepers',
        loadComponent: () =>
          import("./pages/storekeepers-page/storekeepers-page").then(x => x.StorekeepersPage)
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
