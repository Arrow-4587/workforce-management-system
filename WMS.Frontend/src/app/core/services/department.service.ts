import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { DepartmentResponse, CreateDepartment, UpdateDepartment } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAll(): Observable<DepartmentResponse[]> {
    return this.http.get<DepartmentResponse[]>(`${this.baseUrl}${ApiEndpoints.Department.Base}`);
  }

  getById(id: number | string): Observable<DepartmentResponse> {
    return this.http.get<DepartmentResponse>(`${this.baseUrl}${ApiEndpoints.Department.ById(id)}`);
  }

  create(dto: CreateDepartment): Observable<DepartmentResponse> {
    return this.http.post<DepartmentResponse>(`${this.baseUrl}${ApiEndpoints.Department.Base}`, dto);
  }

  update(id: number | string, dto: UpdateDepartment): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}${ApiEndpoints.Department.ById(id)}`, dto);
  }

  delete(id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}${ApiEndpoints.Department.ById(id)}`);
  }

  search(name: string): Observable<DepartmentResponse[]> {
    return this.http.get<DepartmentResponse[]>(`${this.baseUrl}${ApiEndpoints.Department.Search}`, {
      params: { name }
    });
  }
}
