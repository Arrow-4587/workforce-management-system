import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { ProjectResponse, CreateProject, UpdateProject } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAll(): Observable<ProjectResponse[]> {
    return this.http.get<ProjectResponse[]>(`${this.baseUrl}${ApiEndpoints.Project.Base}`);
  }

  getById(id: number | string): Observable<ProjectResponse> {
    return this.http.get<ProjectResponse>(`${this.baseUrl}${ApiEndpoints.Project.ById(id)}`);
  }

  create(dto: CreateProject): Observable<ProjectResponse> {
    return this.http.post<ProjectResponse>(`${this.baseUrl}${ApiEndpoints.Project.Base}`, dto);
  }

  update(id: number | string, dto: UpdateProject): Observable<ProjectResponse> {
    return this.http.put<ProjectResponse>(`${this.baseUrl}${ApiEndpoints.Project.ById(id)}`, dto);
  }

  delete(id: number | string): Observable<string> {
    return this.http.delete(`${this.baseUrl}${ApiEndpoints.Project.ById(id)}`, {
      responseType: 'text'
    });
  }

  getMyProjects(): Observable<ProjectResponse[]> {
    return this.http.get<ProjectResponse[]>(`${this.baseUrl}${ApiEndpoints.Project.MyProjects}`);
  }
}
