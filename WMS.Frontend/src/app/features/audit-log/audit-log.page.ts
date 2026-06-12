import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { AuditLogService } from '../../core/services/audit-log.service';
import { AuditLogResponse } from '../../core/models/wms.models';

const ENTITIES = ['Department', 'Employee', 'Client', 'Project', 'Allocation', 'Leave', 'Announcement', 'Role'];

@Component({
  standalone: true,
  imports: [NgIf, NgFor, NgClass, DatePipe],
  templateUrl: './audit-log.page.html',
  styleUrls: ['./audit-log.page.css']
})
export class AuditLogPageComponent implements OnInit {
  private readonly auditLogService = inject(AuditLogService);

  protected readonly loading = signal(true);
  protected readonly records = signal<AuditLogResponse[]>([]);
  protected readonly selectedEntity = signal('');
  protected readonly page = signal(0);
  protected readonly pageSize = 15;
  protected readonly entities = ENTITIES;

  ngOnInit(): void { this.loadAll(); }

  loadAll(): void {
    this.loading.set(true);
    this.auditLogService.getAll().subscribe({
      next: d => { this.records.set(d ?? []); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  refresh(): void {
    this.selectedEntity.set('');
    this.page.set(0);
    this.loadAll();
  }

  onEntityChange(entity: string): void {
    this.selectedEntity.set(entity);
    this.page.set(0);
    if (!entity) { this.loadAll(); return; }
    this.loading.set(true);
    this.auditLogService.getByEntity(entity).subscribe({
      next: d => { this.records.set(d ?? []); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  pagedItems(): AuditLogResponse[] {
    const start = this.page() * this.pageSize;
    return this.records().slice(start, start + this.pageSize);
  }

  totalPages(): number { return Math.max(1, Math.ceil(this.records().length / this.pageSize)); }
  prevPage(): void { this.page.update(p => Math.max(0, p - 1)); }
  nextPage(): void { this.page.update(p => Math.min(this.totalPages() - 1, p + 1)); }

  countByAction(action: string): number { return this.records().filter(r => r.action === action).length; }

  actionBadge(action: string): string {
    const a = (action ?? '').toLowerCase();
    if (a === 'create') return 'badge--create';
    if (a === 'update') return 'badge--update';
    if (a === 'delete') return 'badge--delete';
    return 'badge--neutral';
  }
}
