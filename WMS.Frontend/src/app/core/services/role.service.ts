import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { CreateRole, RoleResponse, UpdateRole } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAll(): Observable<RoleResponse[]> {
    return this.http.get<RoleResponse[]>(`${this.baseUrl}${ApiEndpoints.Role.Base}`);
  }

  getById(id: number | string): Observable<RoleResponse> {
    return this.http.get<RoleResponse>(`${this.baseUrl}${ApiEndpoints.Role.ById(id)}`);
  }

  create(dto: CreateRole): Observable<RoleResponse> {
    return this.http.post<RoleResponse>(`${this.baseUrl}${ApiEndpoints.Role.Base}`, dto);
  }

  update(id: number | string, dto: UpdateRole): Observable<string> {
    return this.http.put(`${this.baseUrl}${ApiEndpoints.Role.ById(id)}`, dto, { responseType: 'text' });
  }

  delete(id: number | string): Observable<string> {
    return this.http.delete(`${this.baseUrl}${ApiEndpoints.Role.ById(id)}`, { responseType: 'text' });
  }
}
