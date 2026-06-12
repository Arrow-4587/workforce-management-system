import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe, NgFor, NgIf, NgTemplateOutlet } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AttendanceService } from '../../core/services/attendance.service';
import { AuthService } from '../../core/services/auth.service';
import { EmployeeService } from '../../core/services/employee.service';
import { ToastService } from '../../shared/components/toast.service';
import { AttendanceResponse, CheckInRequest, EmployeeResponse } from '../../core/models/wms.models';

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgFor, DatePipe, DecimalPipe, NgTemplateOutlet],
  templateUrl: './attendance.page.html',
  styleUrls: ['./attendance.page.css']
})
export class AttendancePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly attendanceService = inject(AttendanceService);
  private readonly employeeService = inject(EmployeeService);
  private readonly toast = inject(ToastService);
  readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly attendance = signal<AttendanceResponse[]>([]);
  
  // Team Lookup Signals
  protected readonly employees = signal<EmployeeResponse[]>([]);
  protected readonly selectedEmployeeId = signal<string>('');
  protected readonly teamAttendance = signal<AttendanceResponse[]>([]);

  protected readonly form = this.fb.nonNullable.group({
    workMode: ['Office', [Validators.required]]
  });

  ngOnInit(): void {
    if (!this.isRootAdmin()) {
      this.refresh();
    }
    if (this.isAdminOrManager()) {
      this.employeeService.getAll().subscribe(emps => this.employees.set(emps ?? []));
    }
  }

  isRootAdmin(): boolean {
    return this.auth.getUsername()?.toLowerCase() === 'admin';
  }

  isAdminOrManager(): boolean {
    const role = this.auth.getUserRole();
    return role === 'Admin' || role === 'Manager';
  }

  refresh(): void {
    this.attendanceService.getMyAttendance().subscribe(records => this.attendance.set(records ?? []));
  }

  onEmployeeSelect(employeeId: string): void {
    this.selectedEmployeeId.set(employeeId);
    if (!employeeId) {
      this.teamAttendance.set([]);
      return;
    }
    
    this.attendanceService.getByEmployee(employeeId).subscribe({
      next: records => this.teamAttendance.set(records ?? []),
      error: err => this.toast.error('Failed to load records', err?.error)
    });
  }

  checkIn(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.attendanceService.checkIn(this.form.getRawValue() as CheckInRequest).subscribe({
      next: () => {
        this.loading.set(false);
        this.refresh();
        this.toast.success('Checked in', 'Your attendance has been recorded.');
      },
      error: (err: any) => { this.loading.set(false); this.toast.error('Check-in failed', err?.error ?? 'Please try again.'); }
    });
  }

  checkOut(): void {
    this.loading.set(true);
    this.attendanceService.checkOut().subscribe({
      next: () => {
        this.loading.set(false);
        this.refresh();
        this.toast.success('Checked out', 'Your departure has been recorded.');
      },
      error: (err: any) => { this.loading.set(false); this.toast.error('Check-out failed', err?.error ?? 'Please try again.'); }
    });
  }
}
