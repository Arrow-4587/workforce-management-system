import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AllocationService } from '../../core/services/allocation.service';
import { ProjectService } from '../../core/services/project.service';
import { EmployeeService } from '../../core/services/employee.service';
import { ToastService } from '../../shared/components/toast.service';
import { ConfirmService } from '../../shared/components/confirm.service';
import { AuthService } from '../../core/services/auth.service';
import { AllocationResponse, ProjectResponse, EmployeeResponse } from '../../core/models/wms.models';

@Component({
  standalone: true,
  imports: [NgIf, NgFor, NgClass, DatePipe, ReactiveFormsModule],
  templateUrl: './allocation.page.html',
  styleUrls: ['./allocation.page.css']
})
export class AllocationPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly allocationService = inject(AllocationService);
  private readonly projectService = inject(ProjectService);
  private readonly employeeService = inject(EmployeeService);
  private readonly toast = inject(ToastService);
  private readonly confirmSvc = inject(ConfirmService);
  readonly auth = inject(AuthService);

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly editing = signal(false);
  protected readonly records = signal<AllocationResponse[]>([]);
  protected readonly projects = signal<ProjectResponse[]>([]);
  protected readonly employees = signal<EmployeeResponse[]>([]);
  protected readonly selectedProjectId = signal<string>('');

  protected readonly form = this.fb.nonNullable.group({
    employeeId: ['', [Validators.required]],
    projectId: ['', [Validators.required]],
  });

  ngOnInit(): void {
    this.projectService.getAll().subscribe(p => this.projects.set(p ?? []));
    this.employeeService.getAll().subscribe(e => this.employees.set(e ?? []));
  }

  isAdmin(): boolean { return this.auth.getUserRole() === 'Admin'; }

  onProjectChange(projectId: string): void {
    this.selectedProjectId.set(projectId);
    if (projectId) {
      this.loading.set(true);
      this.allocationService.getByProject(Number(projectId)).subscribe({
        next: d => { this.records.set(d ?? []); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
    } else {
      this.records.set([]);
    }
  }

  openCreate(): void {
    this.form.reset({ employeeId: '', projectId: this.selectedProjectId() ? this.selectedProjectId() : '' });
    this.editing.set(true);
  }

  cancelEdit(): void { this.editing.set(false); }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!ctrl && ctrl.touched && ctrl.invalid;
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving.set(true);
    const v = this.form.getRawValue();
    this.allocationService.allocate({ employeeId: Number(v.employeeId), projectId: Number(v.projectId) }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.toast.success('Employee allocated successfully');
        if (this.selectedProjectId()) this.onProjectChange(this.selectedProjectId());
      },
      error: err => { this.saving.set(false); this.toast.error('Allocation failed', err?.error); }
    });
  }

  release(a: AllocationResponse): void {
    this.confirmSvc.open({
      title: 'Release Allocation',
      message: `Are you sure you want to release ${a.employeeName ?? 'this employee'} from ${a.projectName ?? 'this project'}?`,
      confirmText: 'Release',
      cancelText: 'Cancel',
      isDestructive: true,
      onConfirm: () => {
        this.allocationService.release(a.allocationId).subscribe({
          next: () => {
            this.toast.success('Employee released');
            if (this.selectedProjectId()) this.onProjectChange(this.selectedProjectId());
          },
          error: err => this.toast.error('Release failed', err?.error)
        });
      }
    });
  }
}
