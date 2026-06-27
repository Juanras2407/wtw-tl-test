import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from '@angular/core';
import { CommonModule, DatePipe, JsonPipe } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ColumnDef, ActionDef } from './data-table.models';
import { StatusBadgePipe } from '../../pipes/status-badge.pipe';

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    DatePipe,
    JsonPipe,
    StatusBadgePipe,
  ],
  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.scss',
})
export class DataTableComponent implements OnChanges {
  @Input() data: unknown[] = [];
  @Input() columns: ColumnDef[] = [];
  @Input() actions: ActionDef[] = [];
  @Output() rowClick = new EventEmitter<unknown>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  dataSource = new MatTableDataSource<unknown>();

  get displayedColumns(): string[] {
    const cols = this.columns.map((c) => c.key);
    if (this.actions.length > 0) {
      cols.push('actions');
    }
    return cols;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.dataSource.data = this.data;
      // Need to set sort/paginator after view init
      setTimeout(() => {
        if (this.sort) {
          this.dataSource.sort = this.sort;
        }
        if (this.paginator) {
          this.dataSource.paginator = this.paginator;
        }
      });
    }
  }

  getCellValue(row: any, key: string): any {
    // Support nested keys with dot notation
    return key.split('.').reduce<any>((obj, k) => {
      if (obj && typeof obj === 'object') {
        return obj[k];
      }
      return undefined;
    }, row);
  }

  onRowClick(row: unknown): void {
    this.rowClick.emit(row);
  }

  onAction(event: Event, action: ActionDef, row: unknown): void {
    event.stopPropagation();
    action.callback(row);
  }

  isExpanded: Record<number, boolean> = {};

  toggleJson(index: number): void {
    this.isExpanded[index] = !this.isExpanded[index];
  }
}
