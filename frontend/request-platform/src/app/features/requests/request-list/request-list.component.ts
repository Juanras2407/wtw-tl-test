import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { ColumnDef, ActionDef } from '../../../shared/components/data-table/data-table.models';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { RequestService } from '../../../core/services/request.service';
import { Request, RequestFilter, RequestType, RequestStatus } from '../../../core/models/request.model';

@Component({
  selector: 'app-request-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatProgressSpinnerModule,
    DataTableComponent,
  ],
  templateUrl: './request-list.component.html',
  styleUrl: './request-list.component.scss',
})
export class RequestListComponent implements OnInit {
  private readonly requestService = inject(RequestService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);

  requests = signal<Request[]>([]);
  loading = signal(true);

  // Filters
  filterType: RequestType | '' = '';
  filterStatus: RequestStatus | '' = '';
  filterDateFrom: Date | null = null;
  filterDateTo: Date | null = null;
  filterEmployeeName = '';

  readonly requestTypes: { value: RequestType; label: string }[] = [
    { value: 'vacation', label: 'Vacation' },
    { value: 'loan', label: 'Loan' },
    { value: 'permission', label: 'Permission' },
  ];

  readonly statuses: { value: RequestStatus; label: string }[] = [
    { value: 'pending', label: 'Pending' },
    { value: 'approved', label: 'Approved' },
    { value: 'rejected', label: 'Rejected' },
  ];

  readonly columns: ColumnDef[] = [
    { key: 'type', header: 'Type', type: 'badge' },
    { key: 'status', header: 'Status', type: 'badge' },
    { key: 'employeeName', header: 'Employee', type: 'text' },
    { key: 'summary', header: 'Request Summary', type: 'text' },
    { key: 'createdAt', header: 'Created', type: 'date' },
    { key: 'dynamicData', header: 'JSON Data', type: 'json' },
  ];

  readonly actions: ActionDef[] = [
    {
      icon: 'delete_outline',
      tooltip: 'Delete request',
      color: 'warn',
      callback: (row: unknown) => this.onDelete(row as Request),
    },
  ];

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests(): void {
    this.loading.set(true);
    const filters: RequestFilter = {};

    if (this.filterType) filters.type = this.filterType;
    if (this.filterStatus) filters.status = this.filterStatus;
    if (this.filterDateFrom) {
      filters.dateFrom = this.filterDateFrom.toISOString().split('T')[0];
    }
    if (this.filterDateTo) {
      filters.dateTo = this.filterDateTo.toISOString().split('T')[0];
    }
    if (this.filterEmployeeName.trim()) {
      filters.employeeName = this.filterEmployeeName.trim();
    }

    this.requestService.getAll(filters).subscribe({
      next: (data) => {
        // Enrich with summary field and employeeName for display
        const enriched = data.map((req) => {
          const dyn = (req.dynamicData || {}) as Record<string, any>;
          return {
            ...req,
            employeeName: dyn['employeeName'] || 'General Employee',
            summary: this.getSummary(req),
          };
        });
        this.requests.set(enriched);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  onApplyFilters(): void {
    this.loadRequests();
  }

  onClearFilters(): void {
    this.filterType = '';
    this.filterStatus = '';
    this.filterDateFrom = null;
    this.filterDateTo = null;
    this.filterEmployeeName = '';
    this.loadRequests();
  }

  onNewRequest(): void {
    this.router.navigate(['/requests/new']);
  }

  private onDelete(request: Request): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Request',
        message: `Are you sure you want to delete this ${request.type} request? This action cannot be undone.`,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.requestService.delete(request.id).subscribe({
          next: () => {
            this.snackBar.open('Request deleted successfully', 'Close', {
              duration: 3000,
              panelClass: ['success-snackbar'],
            });
            this.loadRequests();
          },
        });
      }
    });
  }

  private getSummary(request: Request): string {
    const data = (request.dynamicData || {}) as Record<string, any>;
    const type = (request.type || '').toLowerCase();
    switch (type) {
      case 'vacation':
        return `From ${data['startDate'] || '?'} to ${data['endDate'] || '?'} (${data['totalDays'] || '?'} days)`;
      case 'loan':
        return `Amount: $${data['amount'] || 0} (${data['installments'] || 0} monthly installments)`;
      case 'permission':
        return `${data['hours'] || 0} hours on ${data['date'] || '?'} - ${data['type'] || 'General'}`;
      default:
        return JSON.stringify(data);
    }
  }
}
