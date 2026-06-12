import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { EmployeeResponse, CreateEmployee, UpdateEmployee } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAll(): Observable<EmployeeResponse[]> {
    return this.http.get<EmployeeResponse[]>(`${this.baseUrl}${ApiEndpoints.Employee.Base}`);
  }

  getById(id: number | string): Observable<EmployeeResponse> {
    return this.http.get<EmployeeResponse>(`${this.baseUrl}${ApiEndpoints.Employee.ById(id)}`);
  }

  create(dto: CreateEmployee): Observable<EmployeeResponse> {
    return this.http.post<EmployeeResponse>(`${this.baseUrl}${ApiEndpoints.Employee.Base}`, dto);
  }

  update(id: number | string, dto: UpdateEmployee): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}${ApiEndpoints.Employee.ById(id)}`, dto);
  }

  delete(id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}${ApiEndpoints.Employee.ById(id)}`);
  }

  search(name: string): Observable<EmployeeResponse[]> {
    return this.http.get<EmployeeResponse[]>(`${this.baseUrl}${ApiEndpoints.Employee.Search}`, {
      params: { name }
    });
  }

  getByDepartment(departmentId: number | string): Observable<EmployeeResponse[]> {
    return this.http.get<EmployeeResponse[]>(`${this.baseUrl}${ApiEndpoints.Employee.ByDepartment(departmentId)}`);
  }

  getByRole(roleId: number | string): Observable<EmployeeResponse[]> {
    return this.http.get<EmployeeResponse[]>(`${this.baseUrl}${ApiEndpoints.Employee.ByRole(roleId)}`);
  }

  getMe(): Observable<EmployeeResponse> {
    return this.http.get<EmployeeResponse>(`${this.baseUrl}${ApiEndpoints.Employee.Me}`);
  }
}
