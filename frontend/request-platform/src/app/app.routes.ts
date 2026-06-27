import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'requests',
    loadChildren: () =>
      import('./features/requests/routes').then((m) => m.REQUEST_ROUTES),
  },
  { path: '', redirectTo: 'requests', pathMatch: 'full' },
  { path: '**', redirectTo: 'requests' },
];
