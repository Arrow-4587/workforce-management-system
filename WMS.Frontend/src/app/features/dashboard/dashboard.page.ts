import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { LeaveService } from '../../core/services/leave.service';
import { AttendanceService } from '../../core/services/attendance.service';
import { AdminDashboardSummary, EmployeeDashboardSummary, ManagerDashboardSummary } from '../../core/models/dashboard.models';
import { LeaveResponse, AttendanceResponse } from '../../core/models/wms.models';

interface KpiCard {
  label: string;
  value: number | string;
  hint: string;
  icon: string;
  color: 'blue' | 'green' | 'amber' | 'purple' | 'teal';
}

@Component({
  standalone: true,
  imports: [NgIf, NgFor, NgClass, DatePipe, RouterLink],
  templateUrl: './dashboard.page.html',
  styleUrls: ['./dashboard.page.css']
})
export class DashboardPageComponent implements OnInit {
  readonly auth = inject(AuthService);
  private readonly dashboard = inject(DashboardService);
  private readonly leaveService = inject(LeaveService);
  private readonly attendanceService = inject(AttendanceService);

  protected readonly loading = signal(true);
  protected readonly adminData = signal<AdminDashboardSummary | null>(null);
  protected readonly managerData = signal<ManagerDashboardSummary | null>(null);
  protected readonly employeeData = signal<EmployeeDashboardSummary | null>(null);
  protected readonly pendingLeaves = signal<LeaveResponse[]>([]);
  protected readonly myAttendance = signal<AttendanceResponse[]>([]);

  ngOnInit(): void {
    const role = this.auth.getUserRole();

    if (role === 'Admin') {
      this.dashboard.getAdminDashboard().subscribe({
        next: data => { this.adminData.set(data); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
    } else if (role === 'Manager') {
      this.dashboard.getManagerDashboard().subscribe({
        next: data => { this.managerData.set(data); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
      this.leaveService.getPending().subscribe(leaves => this.pendingLeaves.set(leaves ?? []));
    } else {
      this.dashboard.getEmployeeDashboard().subscribe({
        next: data => { this.employeeData.set(data); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
      this.attendanceService.getMyAttendance().subscribe(recs => this.myAttendance.set(recs ?? []));
    }
  }

  isAdmin(): boolean { return this.auth.getUserRole() === 'Admin'; }
  isManager(): boolean { return this.auth.getUserRole() === 'Manager'; }
  isEmployee(): boolean { return this.auth.getUserRole() === 'Employee'; }

  username(): string { return this.auth.getUsername() ?? 'there'; }

  greeting(): string {
    const h = new Date().getHours();
    if (h < 12) return 'morning';
    if (h < 17) return 'afternoon';
    return 'evening';
  }

  workModeBadge(mode: string): string {
    const m = (mode ?? '').toLowerCase();
    if (m === 'remote') return 'badge--remote';
    if (m === 'hybrid') return 'badge--hybrid';
    return 'badge--office';
  }

  approveLeave(id: number): void {
    this.leaveService.approve(id).subscribe({
      next: () => this.leaveService.getPending().subscribe(l => this.pendingLeaves.set(l ?? []))
    });
  }

  rejectLeave(id: number): void {
    this.leaveService.reject(id).subscribe({
      next: () => this.leaveService.getPending().subscribe(l => this.pendingLeaves.set(l ?? []))
    });
  }
}
