import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Request, CreateRequestDto, RequestFilter } from '../models/request.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RequestService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/requests`;

  getAll(filters?: RequestFilter): Observable<Request[]> {
    let params = new HttpParams();

    if (filters) {
      if (filters.type) {
        params = params.set('type', filters.type);
      }
      if (filters.status) {
        params = params.set('status', filters.status);
      }
      if (filters.dateFrom) {
        params = params.set('dateFrom', filters.dateFrom);
      }
      if (filters.dateTo) {
        params = params.set('dateTo', filters.dateTo);
      }
      if (filters.employeeName) {
        params = params.set('employeeName', filters.employeeName);
      }
    }

    return this.http.get<Request[]>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Request> {
    return this.http.get<Request>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateRequestDto): Observable<Request> {
    return this.http.post<Request>(this.baseUrl, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
