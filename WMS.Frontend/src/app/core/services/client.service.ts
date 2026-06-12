import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { ClientResponse, CreateClient, UpdateClient } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAll(): Observable<ClientResponse[]> {
    return this.http.get<ClientResponse[]>(`${this.baseUrl}${ApiEndpoints.Client.Base}`);
  }

  getById(id: number | string): Observable<ClientResponse> {
    return this.http.get<ClientResponse>(`${this.baseUrl}${ApiEndpoints.Client.ById(id)}`);
  }

  create(dto: CreateClient): Observable<ClientResponse> {
    return this.http.post<ClientResponse>(`${this.baseUrl}${ApiEndpoints.Client.Base}`, dto);
  }

  update(id: number | string, dto: UpdateClient): Observable<ClientResponse> {
    return this.http.put<ClientResponse>(`${this.baseUrl}${ApiEndpoints.Client.ById(id)}`, dto);
  }

  delete(id: number | string): Observable<string> {
    return this.http.delete(`${this.baseUrl}${ApiEndpoints.Client.ById(id)}`, {
      responseType: 'text'
    });
  }
}
