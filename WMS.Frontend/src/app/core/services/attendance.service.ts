import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { AttendanceResponse, CheckInRequest } from '../models/wms.models';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  checkIn(dto: CheckInRequest): Observable<AttendanceResponse> {
    return this.http.post<AttendanceResponse>(`${this.baseUrl}${ApiEndpoints.Attendance.CheckIn}`, dto);
  }

  checkOut(): Observable<AttendanceResponse> {
    return this.http.post<AttendanceResponse>(`${this.baseUrl}${ApiEndpoints.Attendance.CheckOut}`, {});
  }

  getMyAttendance(): Observable<AttendanceResponse[]> {
    return this.http.get<AttendanceResponse[]>(`${this.baseUrl}${ApiEndpoints.Attendance.My}`);
  }

  getByEmployee(employeeId: number | string): Observable<AttendanceResponse[]> {
    return this.http.get<AttendanceResponse[]>(`${this.baseUrl}${ApiEndpoints.Attendance.ByEmployee(employeeId)}`);
  }

  getMonthly(year: number, month: number): Observable<AttendanceResponse[]> {
    return this.http.get<AttendanceResponse[]>(`${this.baseUrl}${ApiEndpoints.Attendance.Monthly}`, {
      params: { year: year.toString(), month: month.toString() }
    });
  }
}
