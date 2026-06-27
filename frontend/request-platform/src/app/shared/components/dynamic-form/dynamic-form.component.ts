import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormFieldSchema } from '../../../core/models/form-schema.model';

@Component({
  selector: 'app-dynamic-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './dynamic-form.component.html',
  styleUrl: './dynamic-form.component.scss',
})
export class DynamicFormComponent implements OnInit, OnChanges {
  @Input() schema: FormFieldSchema[] = [];
  @Input() initialValues?: Record<string, unknown>;
  @Output() formSubmit = new EventEmitter<Record<string, unknown>>();
  @Output() formCancel = new EventEmitter<void>();

  form!: FormGroup;

  private fb = new FormBuilder();

  ngOnInit(): void {
    this.buildForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['schema'] && !changes['schema'].firstChange) {
      this.buildForm();
    }
  }

  private buildForm(): void {
    const controls: Record<string, unknown[]> = {};

    for (const field of this.schema) {
      const validators = [];
      if (field.required) {
        validators.push(Validators.required);
      }
      if (field.type === 'number' && field.min !== undefined) {
        validators.push(Validators.min(field.min));
      }

      const initialValue = this.initialValues?.[field.key] ?? '';
      controls[field.key] = [initialValue, validators];
    }

    this.form = this.fb.group(controls);
  }

  onSubmit(): void {
    if (this.form.valid) {
      const value = this.form.getRawValue();
      // Convert date objects to ISO strings
      for (const field of this.schema) {
        if (field.type === 'date' && value[field.key] instanceof Date) {
          value[field.key] = (value[field.key] as Date).toISOString().split('T')[0];
        }
      }
      this.formSubmit.emit(value);
    } else {
      this.form.markAllAsTouched();
    }
  }

  onCancel(): void {
    this.formCancel.emit();
  }

  getErrorMessage(key: string): string {
    const control = this.form.get(key);
    if (control?.hasError('required')) {
      return 'This field is required';
    }
    if (control?.hasError('min')) {
      return `Minimum value is ${control.errors?.['min'].min}`;
    }
    return '';
  }
}
