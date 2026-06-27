export interface Request {
  id: string;
  type: RequestType;
  status: RequestStatus;
  dynamicData: Record<string, unknown>;
  createdAt: string;
}

export type RequestType = 'vacation' | 'loan' | 'permission';

export type RequestStatus = 'pending' | 'approved' | 'rejected';

export interface CreateRequestDto {
  type: RequestType;
  dynamicData: Record<string, unknown>;
}

export interface RequestFilter {
  type?: RequestType;
  status?: RequestStatus;
  dateFrom?: string;
  dateTo?: string;
  employeeName?: string;
}

export interface VacationData {
  startDate: string;
  endDate: string;
  reason: string;
}

export interface LoanData {
  amount: number;
  installments: number;
  purpose: string;
}

export interface PermissionData {
  date: string;
  hours: number;
  reason: string;
}
