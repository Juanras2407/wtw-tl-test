import { Routes } from '@angular/router';

export const REQUEST_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./request-list/request-list.component').then(
        (m) => m.RequestListComponent
      ),
  },
  {
    path: 'new',
    loadComponent: () =>
      import('./request-create/request-create.component').then(
        (m) => m.RequestCreateComponent
      ),
  },
];
