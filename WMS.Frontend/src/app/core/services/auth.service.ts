import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEndpoints } from '../constants/api.constants';
import { LoginRequest, LoginResponse, ChangePasswordRequest } from '../models/wms.models';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  
  // Use Signal to track the current user for reactive UI updates
  readonly currentUser = signal<LoginResponse | null>(null);

  constructor() {
    this.loadSession();
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}${ApiEndpoints.Auth.Login}`, request).pipe(
      tap(response => {
        this.saveSession(response);
      })
    );
  }

  changePassword(request: ChangePasswordRequest): Observable<string> {
    return this.http.post(`${environment.apiUrl}${ApiEndpoints.Auth.ChangePassword}`, request, {
      responseType: 'text'
    });
  }

  logout(navigate = true): void {
    this.clearSession();

    if (navigate) {
      this.router.navigate(['/login']);
    }
  }

  isAuthenticated(): boolean {
    return !!this.currentUser();
  }

  getUserRole(): string | null {
    const user = this.currentUser();
    return user ? user.role : null;
  }

  getUsername(): string | null {
    const user = this.currentUser();
    return user ? user.username : null;
  }

  private saveSession(response: LoginResponse): void {
    localStorage.setItem('wms_session', JSON.stringify(response));
    this.currentUser.set(response);
  }

  private loadSession(): void {
    const sessionStr = localStorage.getItem('wms_session');
    if (sessionStr) {
      try {
        const session = JSON.parse(sessionStr) as LoginResponse;
        this.currentUser.set(session);
      } catch (e) {
        this.clearSession();
      }
    }
  }

  private clearSession(): void {
    localStorage.removeItem('wms_session');
    this.currentUser.set(null);
  }
}
