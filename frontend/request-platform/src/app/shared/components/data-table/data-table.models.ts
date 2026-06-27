export interface ColumnDef {
  key: string;
  header: string;
  type?: 'text' | 'date' | 'badge' | 'json';
}

export interface ActionDef {
  icon: string;
  tooltip: string;
  color?: string;
  callback: (row: unknown) => void;
}
