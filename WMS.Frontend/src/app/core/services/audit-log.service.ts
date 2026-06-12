import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { AuditLogResponse } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAll(): Observable<AuditLogResponse[]> {
    return this.http.get<AuditLogResponse[]>(`${this.baseUrl}${ApiEndpoints.AuditLog.Base}`);
  }

  getByEntity(entityName: string): Observable<AuditLogResponse[]> {
    return this.http.get<AuditLogResponse[]>(`${this.baseUrl}${ApiEndpoints.AuditLog.ByEntity(entityName)}`);
  }
}
