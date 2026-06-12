import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { AnnouncementResponse, CreateAnnouncement, UpdateAnnouncement } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getAll(): Observable<AnnouncementResponse[]> {
    return this.http.get<AnnouncementResponse[]>(`${this.baseUrl}${ApiEndpoints.Announcement.Base}`);
  }

  getById(id: number | string): Observable<AnnouncementResponse> {
    return this.http.get<AnnouncementResponse>(`${this.baseUrl}${ApiEndpoints.Announcement.ById(id)}`);
  }

  create(dto: CreateAnnouncement): Observable<AnnouncementResponse> {
    return this.http.post<AnnouncementResponse>(`${this.baseUrl}${ApiEndpoints.Announcement.Base}`, dto);
  }

  update(id: number | string, dto: UpdateAnnouncement): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}${ApiEndpoints.Announcement.ById(id)}`, dto);
  }

  delete(id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}${ApiEndpoints.Announcement.ById(id)}`);
  }
}
