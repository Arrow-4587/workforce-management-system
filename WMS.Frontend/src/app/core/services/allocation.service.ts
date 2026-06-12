import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { AllocationResponse, AllocateEmployee } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class AllocationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  allocate(dto: AllocateEmployee): Observable<AllocationResponse> {
    return this.http.post<AllocationResponse>(`${this.baseUrl}${ApiEndpoints.Allocation.Base}`, dto);
  }

  release(allocationId: number | string): Observable<string> {
    return this.http.delete(`${this.baseUrl}${ApiEndpoints.Allocation.ById(allocationId)}`, {
      responseType: 'text'
    });
  }

  getByProject(projectId: number | string): Observable<AllocationResponse[]> {
    return this.http.get<AllocationResponse[]>(`${this.baseUrl}${ApiEndpoints.Allocation.ByProject(projectId)}`);
  }

  getByEmployee(employeeId: number | string): Observable<AllocationResponse[]> {
    return this.http.get<AllocationResponse[]>(`${this.baseUrl}${ApiEndpoints.Allocation.ByEmployee(employeeId)}`);
  }

  getMyAllocations(): Observable<AllocationResponse[]> {
    return this.http.get<AllocationResponse[]>(`${this.baseUrl}${ApiEndpoints.Allocation.MyProjects}`);
  }
}
