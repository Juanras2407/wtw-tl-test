import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'An unexpected error occurred';

      if (error.status === 0) {
        message = 'Unable to connect to the server. Please check your connection.';
      } else if (error.status === 404) {
        message = 'The requested resource was not found.';
      } else if (error.status === 400) {
        message = error.error?.message || 'Invalid request. Please check your data.';
      } else if (error.status === 500) {
        message = 'Internal server error. Please try again later.';
      } else if (error.error?.message) {
        message = error.error.message;
      }

      snackBar.open(message, 'Dismiss', {
        duration: 5000,
        horizontalPosition: 'end',
        verticalPosition: 'top',
        panelClass: ['error-snackbar'],
      });

      return throwError(() => error);
    })
  );
};
