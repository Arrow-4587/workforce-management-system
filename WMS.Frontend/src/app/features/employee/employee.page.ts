import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmployeeService } from '../../core/services/employee.service';
import { DepartmentService } from '../../core/services/department.service';
import { RoleService } from '../../core/services/role.service';
import { ToastService } from '../../shared/components/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { EmployeeResponse, DepartmentResponse, RoleResponse } from '../../core/models/wms.models';

@Component({
  standalone: true,
  imports: [NgIf, NgFor, NgClass, ReactiveFormsModule],
  templateUrl: './employee.page.html',
  styleUrls: ['./employee.page.css']
})
export class EmployeePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly employeeService = inject(EmployeeService);
  private readonly departmentService = inject(DepartmentService);
  private readonly roleService = inject(RoleService);
  private readonly toast = inject(ToastService);
  readonly auth = inject(AuthService);

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly editing = signal(false);
  protected readonly mode = signal<'create' | 'edit'>('create');
  protected readonly searchTerm = signal('');
  protected readonly page = signal(0);
  protected readonly pageSize = 10;

  protected readonly records = signal<EmployeeResponse[]>([]);
  protected readonly departments = signal<DepartmentResponse[]>([]);
  protected readonly roles = signal<RoleResponse[]>([]);
  protected readonly selectedEmployee = signal<EmployeeResponse | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(80)]],
    phoneNumber: ['', [Validators.required, Validators.maxLength(15)]],
    gender: ['M', [Validators.required]],
    dob: ['', [Validators.required]],
    doj: ['', [Validators.required]],
    departmentId: ['', [Validators.required]],
    roleId: ['', [Validators.required]],
    status: ['Active', [Validators.required]],
  }, { validators: this.ageValidator });

  ageValidator(group: import('@angular/forms').AbstractControl): import('@angular/forms').ValidationErrors | null {
    const dob = group.get('dob')?.value;
    const doj = group.get('doj')?.value;
    if (dob && doj) {
      const dobDate = new Date(dob);
      const dojDate = new Date(doj);
      let age = dojDate.getFullYear() - dobDate.getFullYear();
      const m = dojDate.getMonth() - dobDate.getMonth();
      if (m < 0 || (m === 0 && dojDate.getDate() < dobDate.getDate())) {
        age--;
      }
      if (age < 18) {
        return { underage: true };
      }
      if (dojDate < dobDate) {
        return { invalidDoj: true };
      }
    }
    return null;
  }

  ngOnInit(): void {
    this.loadAll();
    this.departmentService.getAll().subscribe(d => this.departments.set(d ?? []));
    this.roleService.getAll().subscribe(r => this.roles.set(r ?? []));
  }

  isAdmin(): boolean { return this.auth.getUserRole() === 'Admin'; }
  initials(e: EmployeeResponse): string { return `${e.firstName[0]}${e.lastName[0]}`.toUpperCase(); }

  pagedItems(): EmployeeResponse[] {
    const start = this.page() * this.pageSize;
    return this.records().slice(start, start + this.pageSize);
  }

  totalPages(): number { return Math.max(1, Math.ceil(this.records().length / this.pageSize)); }
  prevPage(): void { this.page.update(p => Math.max(0, p - 1)); }
  nextPage(): void { this.page.update(p => Math.min(this.totalPages() - 1, p + 1)); }

  loadAll(): void {
    this.loading.set(true);
    this.employeeService.getAll().subscribe({
      next: data => { this.records.set(data ?? []); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  onSearch(term: string): void {
    this.searchTerm.set(term);
    this.page.set(0);
    if (!term.trim()) { this.loadAll(); return; }
    this.employeeService.search(term).subscribe(data => this.records.set(data ?? []));
  }

  openCreate(): void {
    this.mode.set('create');
    this.selectedEmployee.set(null);
    this.form.reset({ firstName: '', lastName: '', email: '', phoneNumber: '', gender: 'M', dob: '', doj: '', departmentId: '', roleId: '', status: 'Active' });
    this.editing.set(true);
  }

  openEdit(emp: EmployeeResponse): void {
    this.mode.set('edit');
    this.selectedEmployee.set(emp);
    this.form.patchValue({
      firstName: emp.firstName,
      lastName: emp.lastName,
      email: emp.email,
      phoneNumber: emp.phoneNumber,
      gender: emp.gender || 'M',
      dob: emp.dob?.slice(0, 10) ?? '',
      doj: emp.doj?.slice(0, 10) ?? '',
      departmentId: String(emp.departmentId),
      roleId: String(emp.roleId),
      status: emp.status,
    });
    this.editing.set(true);
  }

  cancelEdit(): void { this.editing.set(false); }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!ctrl && ctrl.touched && ctrl.invalid;
  }

  private extractError(err: any): string {
    if (err?.error) {
      if (typeof err.error === 'string') {
        if (err.error.includes('<html')) {
          const match = err.error.match(/System\.Exception: (.*?)(?:\r|\n|<)/);
          if (match && match[1]) return match[1];
          return 'A server error occurred. Please try again.';
        }
        return err.error;
      } else if (err.error.message) {
        return err.error.message;
      }
    }
    return err?.message ?? 'Please try again.';
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving.set(true);
    const v = this.form.getRawValue();

    if (this.mode() === 'create') {
      const payload = { ...v, departmentId: Number(v.departmentId), roleId: Number(v.roleId) };
      this.employeeService.create(payload as any).subscribe({
        next: () => { this.saving.set(false); this.editing.set(false); this.loadAll(); this.toast.success('Employee created', 'New employee has been added.'); },
        error: err => { this.saving.set(false); this.toast.error('Create failed', this.extractError(err)); }
      });
    } else {
      const id = this.selectedEmployee()!.employeeId;
      const payload = { ...v, employeeId: id, departmentId: Number(v.departmentId), roleId: Number(v.roleId) };
      this.employeeService.update(id, payload as any).subscribe({
        next: () => { this.saving.set(false); this.editing.set(false); this.loadAll(); this.toast.success('Employee updated', 'Changes have been saved.'); },
        error: err => { this.saving.set(false); this.toast.error('Update failed', this.extractError(err)); }
      });
    }
  }

  remove(emp: EmployeeResponse): void {
    if (!confirm(`Delete ${emp.firstName} ${emp.lastName}? This cannot be undone.`)) return;
    this.employeeService.delete(emp.employeeId).subscribe({
      next: () => { this.loadAll(); this.toast.success('Employee deleted'); },
      error: err => this.toast.error('Delete failed', this.extractError(err))
    });
  }
}
