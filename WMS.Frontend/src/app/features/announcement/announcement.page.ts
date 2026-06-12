import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AnnouncementService } from '../../core/services/announcement.service';
import { ToastService } from '../../shared/components/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { AnnouncementResponse } from '../../core/models/wms.models';

@Component({
  standalone: true,
  imports: [NgIf, NgFor, NgClass, DatePipe, ReactiveFormsModule],
  templateUrl: './announcement.page.html',
  styleUrls: ['./announcement.page.css']
})
export class AnnouncementPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly announcementService = inject(AnnouncementService);
  private readonly toast = inject(ToastService);
  readonly auth = inject(AuthService);

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly editing = signal(false);
  protected readonly mode = signal<'create' | 'edit'>('create');
  protected readonly records = signal<AnnouncementResponse[]>([]);
  protected readonly selectedAnnouncement = signal<AnnouncementResponse | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    message: ['', [Validators.required, Validators.maxLength(2000)]],
    isActive: [true],
  });

  ngOnInit(): void { this.loadAll(); }

  isAdmin(): boolean { return this.auth.getUserRole() === 'Admin'; }
  activeAnnouncements(): AnnouncementResponse[] { return this.records().filter(a => a.isActive); }

  loadAll(): void {
    this.loading.set(true);
    this.announcementService.getAll().subscribe({
      next: d => { this.records.set(d ?? []); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  openCreate(): void {
    this.mode.set('create');
    this.selectedAnnouncement.set(null);
    this.form.reset({ title: '', message: '', isActive: true });
    this.editing.set(true);
  }

  openEdit(a: AnnouncementResponse): void {
    this.mode.set('edit');
    this.selectedAnnouncement.set(a);
    this.form.patchValue({ title: a.title, message: a.message, isActive: a.isActive });
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

    if (this.mode() === 'create') {
      this.announcementService.create({ title: v.title, message: v.message, isActive: v.isActive }).subscribe({
        next: () => { this.saving.set(false); this.editing.set(false); this.loadAll(); this.toast.success('Announcement published'); },
        error: err => { this.saving.set(false); this.toast.error('Publish failed', err?.error); }
      });
    } else {
      const id = this.selectedAnnouncement()!.announcementId;
      this.announcementService.update(id, { announcementId: id, ...v }).subscribe({
        next: () => { this.saving.set(false); this.editing.set(false); this.loadAll(); this.toast.success('Announcement updated'); },
        error: err => { this.saving.set(false); this.toast.error('Update failed', err?.error); }
      });
    }
  }

  remove(a: AnnouncementResponse): void {
    if (!confirm(`Delete announcement "${a.title}"?`)) return;
    this.announcementService.delete(a.announcementId).subscribe({
      next: () => { this.loadAll(); this.toast.success('Announcement deleted'); },
      error: err => this.toast.error('Delete failed', err?.error)
    });
  }
}
