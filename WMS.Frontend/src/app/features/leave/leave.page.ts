import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { LeaveService } from '../../core/services/leave.service';
import { ToastService } from '../../shared/components/toast.service';
import { ConfirmService } from '../../shared/components/confirm.service';
import { LeaveResponse, ApplyLeaveRequest } from '../../core/models/wms.models';

const dateRangeValidator: ValidatorFn = control => {
  const fromDate = control.get('fromDate')?.value;
  const toDate = control.get('toDate')?.value;
  if (!fromDate || !toDate) {
    return null;
  }

  return new Date(toDate) < new Date(fromDate) ? { invalidDateRange: true } : null;
};

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, NgFor, NgClass, DatePipe],
  templateUrl: './leave.page.html',
  styleUrls: ['./leave.page.css']
})
export class LeavePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly leaveService = inject(LeaveService);
  private readonly toast = inject(ToastService);
  private readonly confirmSvc = inject(ConfirmService);
  readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly creating = signal(false);
  protected readonly myLeaves = signal<LeaveResponse[]>([]);
  protected readonly pendingLeaves = signal<LeaveResponse[]>([]);

  protected readonly form = this.fb.nonNullable.group({
    leaveType: ['', [Validators.required]],
    reason: ['', [Validators.maxLength(500)]],
    fromDate: ['', [Validators.required]],
    toDate: ['', [Validators.required]]
  }, { validators: dateRangeValidator });

  ngOnInit(): void {
    this.refresh();
  }

  isManager(): boolean {
    return this.auth.getUserRole() === 'Manager';
  }

  isAdmin(): boolean {
    return this.auth.getUserRole() === 'Admin';
  }

  isRootAdmin(): boolean {
    return this.auth.getUsername()?.toLowerCase() === 'admin';
  }

  openCreate(): void {
    this.creating.set(true);
  }

  cancelCreate(): void {
    this.creating.set(false);
    this.form.reset();
  }

  refresh(): void {
    if (this.auth.getUserRole() !== 'Admin') {
      this.leaveService.getMyLeaves().subscribe(records => this.myLeaves.set(records ?? []));
    }

    if (this.isAdmin() || this.isManager()) {
      this.leaveService.getPending().subscribe(records => this.pendingLeaves.set(records ?? []));
    }
  }

  applyLeave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const payload = this.form.getRawValue() as ApplyLeaveRequest;
    this.leaveService.apply(payload).subscribe({
      next: () => {
        this.loading.set(false);
        this.form.reset({ leaveType: '', reason: '', fromDate: '', toDate: '' });
        this.creating.set(false);
        this.refresh();
        this.toast.success('Leave applied', 'Your request has been submitted.');
      },
      error: (err: any) => { this.loading.set(false); this.toast.error('Apply failed', err?.error ?? 'Please try again.'); }
    });
  }

  cancelLeave(leaveId: number): void {
    this.confirmSvc.open({
      title: 'Cancel Leave',
      message: 'Are you sure you want to cancel this leave request?',
      confirmText: 'Yes, cancel',
      cancelText: 'No',
      isDestructive: true,
      onConfirm: () => {
        this.leaveService.cancel(leaveId).subscribe({
          next: () => { this.refresh(); this.toast.success('Leave cancelled'); },
          error: (err: any) => this.toast.error('Cancel failed', err?.error)
        });
      }
    });
  }

  approve(leaveId: number): void {
    this.confirmSvc.open({
      title: 'Approve Leave',
      message: 'Are you sure you want to approve this leave request?',
      confirmText: 'Approve',
      cancelText: 'Cancel',
      isDestructive: false,
      onConfirm: () => {
        this.leaveService.approve(leaveId).subscribe({
          next: () => { this.refresh(); this.toast.success('Leave approved'); },
          error: (err: any) => this.toast.error('Approve failed', err?.error)
        });
      }
    });
  }

  reject(leaveId: number): void {
    this.confirmSvc.open({
      title: 'Reject Leave',
      message: 'Are you sure you want to reject this leave request?',
      confirmText: 'Reject',
      cancelText: 'Cancel',
      isDestructive: true,
      onConfirm: () => {
        this.leaveService.reject(leaveId).subscribe({
          next: () => { this.refresh(); this.toast.success('Leave rejected'); },
          error: (err: any) => this.toast.error('Reject failed', err?.error)
        });
      }
    });
  }

  statusBadge(status: string): string {
    const s = (status ?? '').toLowerCase();
    if (s === 'approved') return 'badge--approved';
    if (s === 'rejected') return 'badge--rejected';
    return 'badge--pending';
  }
}
