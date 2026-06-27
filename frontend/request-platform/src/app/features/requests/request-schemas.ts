import { FormFieldSchema } from '../../core/models/form-schema.model';
import { RequestType } from '../../core/models/request.model';

export const REQUEST_TYPE_SCHEMAS: Record<RequestType, FormFieldSchema[]> = {
  vacation: [
    {
      key: 'employeeName',
      label: 'Employee Name',
      type: 'text',
      required: true,
      placeholder: 'Enter your full name',
    },
    {
      key: 'startDate',
      label: 'Start Date',
      type: 'date',
      required: true,
    },
    {
      key: 'endDate',
      label: 'End Date',
      type: 'date',
      required: true,
    },
    {
      key: 'reason',
      label: 'Reason',
      type: 'textarea',
      required: true,
      placeholder: 'Describe the reason for your vacation request',
    },
  ],
  loan: [
    {
      key: 'employeeName',
      label: 'Employee Name',
      type: 'text',
      required: true,
      placeholder: 'Enter your full name',
    },
    {
      key: 'amount',
      label: 'Loan Amount ($)',
      type: 'number',
      required: true,
      min: 1,
      placeholder: 'Enter loan amount',
    },
    {
      key: 'installments',
      label: 'Number of Installments',
      type: 'number',
      required: true,
      min: 1,
      placeholder: 'Number of monthly installments',
    },
    {
      key: 'purpose',
      label: 'Purpose',
      type: 'textarea',
      required: true,
      placeholder: 'Describe the purpose of the loan',
    },
  ],
  permission: [
    {
      key: 'employeeName',
      label: 'Employee Name',
      type: 'text',
      required: true,
      placeholder: 'Enter your full name',
    },
    {
      key: 'date',
      label: 'Date',
      type: 'date',
      required: true,
    },
    {
      key: 'hours',
      label: 'Hours Requested',
      type: 'number',
      required: true,
      min: 1,
      placeholder: 'Number of hours',
    },
    {
      key: 'reason',
      label: 'Reason',
      type: 'textarea',
      required: true,
      placeholder: 'Describe the reason for your permission request',
    },
  ],
};
