import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { AdminDashboardSummary, EmployeeDashboardSummary, ManagerDashboardSummary } from '../models/dashboard.models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAdminDashboard(): Observable<AdminDashboardSummary> {
    return this.http.get<AdminDashboardSummary>(`${this.baseUrl}${ApiEndpoints.Dashboard.Admin}`);
  }

  getManagerDashboard(): Observable<ManagerDashboardSummary> {
    return this.http.get<ManagerDashboardSummary>(`${this.baseUrl}${ApiEndpoints.Dashboard.Manager}`);
  }

  getEmployeeDashboard(): Observable<EmployeeDashboardSummary> {
    return this.http.get<EmployeeDashboardSummary>(`${this.baseUrl}${ApiEndpoints.Dashboard.Employee}`);
  }
}
