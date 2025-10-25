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
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
