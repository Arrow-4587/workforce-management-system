import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { ChangePasswordRequest, ProfileResponse, UpdateProfileRequest } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  getProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.baseUrl}${ApiEndpoints.Profile.Base}`);
  }

  updateProfile(dto: UpdateProfileRequest): Observable<string> {
    return this.http.put(`${this.baseUrl}${ApiEndpoints.Profile.Base}`, dto, { responseType: 'text' });
  }

  changePassword(dto: ChangePasswordRequest): Observable<string> {
    return this.http.put(`${this.baseUrl}${ApiEndpoints.Profile.ChangePassword}`, dto, { responseType: 'text' });
  }
}
