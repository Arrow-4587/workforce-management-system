import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectService } from '../../core/services/project.service';
import { ClientService } from '../../core/services/client.service';
import { EmployeeService } from '../../core/services/employee.service';
import { ToastService } from '../../shared/components/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { ProjectResponse, ClientResponse, EmployeeResponse, AllocationResponse } from '../../core/models/wms.models';
import { AllocationService } from '../../core/services/allocation.service';

@Component({
  standalone: true,
  imports: [NgIf, NgFor, NgClass, DatePipe, ReactiveFormsModule],
  templateUrl: './project.page.html',
  styleUrls: ['./project.page.css']
})
export class ProjectPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly projectService = inject(ProjectService);
  private readonly clientService = inject(ClientService);
  private readonly employeeService = inject(EmployeeService);
  private readonly toast = inject(ToastService);
  readonly auth = inject(AuthService);
  private readonly allocationService = inject(AllocationService);

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly editing = signal(false);
  protected readonly mode = signal<'create' | 'edit'>('create');
  protected readonly searchTerm = signal('');
  protected readonly page = signal(0);
  protected readonly pageSize = 10;

  protected readonly records = signal<ProjectResponse[]>([]);
  protected readonly clients = signal<ClientResponse[]>([]);
  protected readonly managers = signal<EmployeeResponse[]>([]);
  protected readonly selectedProject = signal<ProjectResponse | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    projectName: ['', [Validators.required, Validators.maxLength(100)]],
    clientId: ['', [Validators.required]],
    managerId: ['', [Validators.required]],
    startDate: ['', [Validators.required]],
    endDate: [''],
    status: ['Active'],
  });

  ngOnInit(): void {
    if (this.isEmployee()) {
      this.loadEmployeeProjects();
    } else {
      this.loadAll();
      this.clientService.getAll().subscribe(c => this.clients.set(c ?? []));
      this.employeeService.getAll().subscribe(e => {
        this.managers.set((e ?? []).filter(emp => emp.roleName === 'Manager'));
      });
    }
  }

  isAdmin(): boolean { return this.auth.getUserRole() === 'Admin'; }
  isEmployee(): boolean { return this.auth.getUserRole() === 'Employee'; }

  pagedItems(): ProjectResponse[] {
    const term = this.searchTerm().toLowerCase();
    const filtered = term ? this.records().filter(p => 
      (p.projectName ?? '').toLowerCase().includes(term) || 
      (p.clientName ?? '').toLowerCase().includes(term) ||
      (p.managerName ?? '').toLowerCase().includes(term)
    ) : this.records();
    const start = this.page() * this.pageSize;
    return filtered.slice(start, start + this.pageSize);
  }

  totalPages(): number {
    const term = this.searchTerm().toLowerCase();
    const count = term ? this.records().filter(p => 
      (p.projectName ?? '').toLowerCase().includes(term) || 
      (p.clientName ?? '').toLowerCase().includes(term) ||
      (p.managerName ?? '').toLowerCase().includes(term)
    ).length : this.records().length;
    return Math.max(1, Math.ceil(count / this.pageSize));
  }

  prevPage(): void { this.page.update(p => Math.max(0, p - 1)); }
  nextPage(): void { this.page.update(p => Math.min(this.totalPages() - 1, p + 1)); }

  onSearch(term: string): void { this.searchTerm.set(term); this.page.set(0); }

  loadAll(): void {
    this.loading.set(true);
    this.projectService.getAll().subscribe({
      next: d => { this.records.set(d ?? []); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  loadEmployeeProjects(): void {
    this.loading.set(true);
    this.allocationService.getMyAllocations().subscribe({
      next: (allocations: AllocationResponse[]) => {
        const mappedProjects: ProjectResponse[] = allocations.map(a => ({
          projectId: a.projectId,
          projectName: a.projectName || a.project?.projectName || 'Unknown',
          clientId: 0,
          managerId: 0,
          managerName: a.projectManagerName || a.project?.managerName || 'Unknown',
          status: a.project?.status || 'Active',
          startDate: a.allocatedOn,
          clientName: a.clientName || a.project?.clientName || '-'
        }));
        
        // Remove duplicate projects
        const uniqueProjects = Array.from(new Map(mappedProjects.map(p => [p.projectId, p])).values());
        
        this.records.set(uniqueProjects);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  statusBadge(status: string): string {
    const s = (status ?? '').toLowerCase();
    if (s === 'active') return 'badge--active';
    if (s === 'completed') return 'badge--completed';
    if (s === 'onhold') return 'badge--onhold';
    return 'badge--neutral';
  }

  openCreate(): void {
    this.mode.set('create');
    this.selectedProject.set(null);
    this.form.reset({ projectName: '', clientId: '', managerId: '', startDate: '', endDate: '', status: 'Active' });
    this.editing.set(true);
  }

  openEdit(p: ProjectResponse): void {
    this.mode.set('edit');
    this.selectedProject.set(p);
    this.form.patchValue({
      projectName: p.projectName,
      clientId: String(p.clientId),
      managerId: String(p.managerId),
      startDate: p.startDate?.slice(0, 10) ?? '',
      endDate: p.endDate?.slice(0, 10) ?? '',
      status: p.status,
    });
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
    const payload = { ...v, clientId: Number(v.clientId), managerId: Number(v.managerId), endDate: v.endDate || undefined };

    if (this.mode() === 'create') {
      this.projectService.create(payload as any).subscribe({
        next: () => { this.saving.set(false); this.editing.set(false); this.loadAll(); this.toast.success('Project created'); },
        error: err => { this.saving.set(false); this.toast.error('Create failed', err?.error); }
      });
    } else {
      const id = this.selectedProject()!.projectId;
      this.projectService.update(id, { ...payload, projectId: id } as any).subscribe({
        next: () => { this.saving.set(false); this.editing.set(false); this.loadAll(); this.toast.success('Project updated'); },
        error: err => { this.saving.set(false); this.toast.error('Update failed', err?.error); }
      });
    }
  }

  remove(p: ProjectResponse): void {
    if (!confirm(`Delete project "${p.projectName}"?`)) return;
    this.projectService.delete(p.projectId).subscribe({
      next: () => { this.loadAll(); this.toast.success('Project deleted'); },
      error: err => this.toast.error('Delete failed', err?.error)
    });
  }
}
