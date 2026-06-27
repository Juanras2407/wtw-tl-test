export interface FormFieldSchema {
  key: string;
  label: string;
  type: 'text' | 'number' | 'date' | 'textarea' | 'select';
  required: boolean;
  options?: SelectOption[];
  placeholder?: string;
  min?: number;
  max?: number;
}

export interface SelectOption {
  value: string;
  label: string;
}
