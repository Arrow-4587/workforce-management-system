import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { LeaveResponse, ApplyLeaveRequest } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  apply(dto: ApplyLeaveRequest): Observable<LeaveResponse> {
    return this.http.post<LeaveResponse>(`${this.baseUrl}${ApiEndpoints.Leave.Apply}`, dto);
  }

  cancel(id: number | string): Observable<string> {
    return this.http.delete(`${this.baseUrl}${ApiEndpoints.Leave.Cancel(id)}`, {
      responseType: 'text'
    });
  }

  getMyLeaves(): Observable<LeaveResponse[]> {
    return this.http.get<LeaveResponse[]>(`${this.baseUrl}${ApiEndpoints.Leave.My}`);
  }

  getPending(): Observable<LeaveResponse[]> {
    return this.http.get<LeaveResponse[]>(`${this.baseUrl}${ApiEndpoints.Leave.Pending}`);
  }

  approve(id: number | string): Observable<string> {
    return this.http.post(`${this.baseUrl}${ApiEndpoints.Leave.Approve(id)}`, {}, {
      responseType: 'text'
    });
  }

  reject(id: number | string): Observable<string> {
    return this.http.post(`${this.baseUrl}${ApiEndpoints.Leave.Reject(id)}`, {}, {
      responseType: 'text'
    });
  }
}
