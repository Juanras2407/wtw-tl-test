import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DynamicFormComponent } from '../../../shared/components/dynamic-form/dynamic-form.component';
import { RequestService } from '../../../core/services/request.service';
import { RequestType } from '../../../core/models/request.model';
import { FormFieldSchema } from '../../../core/models/form-schema.model';
import { REQUEST_TYPE_SCHEMAS } from '../request-schemas';

@Component({
  selector: 'app-request-create',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    DynamicFormComponent,
  ],
  templateUrl: './request-create.component.html',
  styleUrl: './request-create.component.scss',
})
export class RequestCreateComponent {
  private readonly requestService = inject(RequestService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  selectedType = signal<RequestType | null>(null);
  currentSchema = signal<FormFieldSchema[]>([]);
  submitting = signal(false);

  readonly requestTypes: { value: RequestType; label: string; icon: string }[] = [
    { value: 'vacation', label: 'Vacation', icon: 'beach_access' },
    { value: 'loan', label: 'Loan', icon: 'account_balance' },
    { value: 'permission', label: 'Permission', icon: 'schedule' },
  ];

  onTypeSelected(type: RequestType): void {
    this.selectedType.set(type);
    this.currentSchema.set(REQUEST_TYPE_SCHEMAS[type]);
  }

  onFormSubmit(formData: Record<string, unknown>): void {
    const type = this.selectedType();
    if (!type) return;

    this.submitting.set(true);
    this.requestService.create({ type, dynamicData: formData }).subscribe({
      next: () => {
        this.snackBar.open('Request created successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar'],
        });
        this.router.navigate(['/requests']);
      },
      error: () => {
        this.submitting.set(false);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/requests']);
  }
}
