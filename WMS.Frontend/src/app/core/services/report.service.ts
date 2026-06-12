import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/report`;

  downloadReport(reportType: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/export/${reportType}`, {
      responseType: 'blob'
    });
  }
}
