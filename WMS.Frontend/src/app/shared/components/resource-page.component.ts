import { NgFor, NgIf, NgSwitch, NgSwitchCase, NgSwitchDefault } from '@angular/common';
import { Component, Injector, OnInit, computed, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ResourceFieldConfig, ResourcePageConfig } from '../../core/models/resource-page.models';
import { ToastService } from './toast.service';
import { ConfirmService } from './confirm.service';

@Component({
  selector: 'app-resource-page',
  standalone: true,
  imports: [NgIf, NgFor, NgSwitch, NgSwitchCase, NgSwitchDefault, ReactiveFormsModule],
  templateUrl: './resource-page.component.html',
  styleUrls: ['./resource-page.component.css']
})
export class ResourcePageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly injector: Injector = inject(Injector);
  private readonly toast = inject(ToastService);
  private readonly confirmSvc = inject(ConfirmService);
  readonly config = input.required<ResourcePageConfig<any, any>>();

  private serviceInstance: any;
  protected loading = signal(true);
  protected saving = signal(false);
  protected editing = signal(false);
  protected mode = signal<'create' | 'edit'>('create');
  protected searchTerm = signal('');
  protected pageIndex = signal(0);
  protected records = signal<any[]>([]);
  protected selectedRecord = signal<any | null>(null);
  protected pageSize = computed(() => this.config().pageSize ?? 10);
  protected form = this.fb.group({});

  ngOnInit(): void {
    const config = this.config();
    this.serviceInstance = this.injector.get(config.service as any);
    this.buildForm(config.fields, config.emptyForm());
    this.loadRecords();
  }

  private buildForm(fields: ResourceFieldConfig<any>[], initialValue: Record<string, any>): void {
    const controls: Record<string, any> = {};

    for (const field of fields) {
      const validators = [];
      if (field.required) validators.push(Validators.required);
      if (field.type === 'email') validators.push(Validators.email);
      if (field.maxLength) validators.push(Validators.maxLength(field.maxLength));
      if (field.minLength) validators.push(Validators.minLength(field.minLength));
      if (field.pattern) validators.push(Validators.pattern(field.pattern));
      controls[field.name] = [initialValue[field.name], validators];
    }

    this.form = this.fb.group(controls);
    this.form.patchValue(initialValue);
  }

  checkboxFields(): ResourceFieldConfig<any>[] {
    return this.config().fields.filter(field => field.type === 'checkbox');
  }

  canCreate(): boolean {
    return this.config().canCreate !== false && !!this.config().createMethod;
  }

  canEdit(): boolean {
    return this.config().canEdit !== false && !!this.config().updateMethod;
  }

  canDelete(): boolean {
    return this.config().canDelete !== false && !!this.config().deleteMethod;
  }

  hasActions(): boolean {
    return this.canEdit() || this.canDelete();
  }

  previousPage(): void {
    this.pageIndex.set(Math.max(0, this.pageIndex() - 1));
  }

  nextPage(): void {
    this.pageIndex.set(Math.min(this.totalPages() - 1, this.pageIndex() + 1));
  }

  loadRecords(): void {
    this.loading.set(true);
    const config = this.config();
    const result = this.serviceInstance[config.loadMethod]();

    result.subscribe({
      next: (items: any[]) => {
        this.records.set(items ?? []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onSearchInput(value: string): void {
    this.searchTerm.set(value);
    this.pageIndex.set(0);
  }

  search(): void {
    const config = this.config();
    const term = this.searchTerm().trim();

    if (!term) {
      this.loadRecords();
      return;
    }

    if (config.searchMethod) {
      this.loading.set(true);
      this.serviceInstance[config.searchMethod](term).subscribe({
        next: (items: any[]) => {
          this.records.set(items ?? []);
          this.loading.set(false);
          this.pageIndex.set(0);
        },
        error: () => this.loading.set(false)
      });
    }
  }

  visibleItems(): any[] {
    const term = this.searchTerm().trim().toLowerCase();
    if (!this.config().searchMethod && term) {
      return this.records().filter(record => JSON.stringify(record).toLowerCase().includes(term));
    }
    return this.records();
  }

  pagedItems(): any[] {
    const start = this.pageIndex() * this.pageSize();
    return this.visibleItems().slice(start, start + this.pageSize());
  }

  totalPages(): number {
    return Math.max(1, Math.ceil(this.visibleItems().length / this.pageSize()));
  }

  startCreate(): void {
    const config = this.config();
    this.mode.set('create');
    this.selectedRecord.set(null);
    this.buildForm(config.fields, config.emptyForm());
    this.editing.set(true);
  }

  startEdit(record: any): void {
    const config = this.config();
    this.mode.set('edit');
    this.selectedRecord.set(record);
    this.buildForm(config.fields, config.toForm ? config.toForm(record) : record);
    this.editing.set(true);
  }

  cancel(): void {
    this.editing.set(false);
    this.selectedRecord.set(null);
  }

  save(): void {
    const config = this.config();
    const payload = this.form.getRawValue();

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const operation = this.mode() === 'create'
      ? config.createMethod && this.serviceInstance[config.createMethod](config.toCreatePayload ? config.toCreatePayload(payload) : payload)
      : config.updateMethod && this.serviceInstance[config.updateMethod](this.selectedRecord()?.[config.idKey], config.toUpdatePayload ? config.toUpdatePayload(payload) : payload);

    if (!operation) {
      this.saving.set(false);
      return;
    }

    const isCreate = this.mode() === 'create';
    operation.subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.selectedRecord.set(null);
        this.loadRecords();
        this.toast.success(isCreate ? 'Record created' : 'Record updated', isCreate ? 'New entry has been added.' : 'Changes have been saved.');
      },
      error: (err: any) => { this.saving.set(false); this.toast.error(isCreate ? 'Create failed' : 'Update failed', err?.error ?? 'Please try again.'); }
    });
  }

  remove(record: any): void {
    const config = this.config();
    if (!config.deleteMethod) {
      return;
    }

    this.confirmSvc.open({
      title: `Delete ${config.title}`,
      message: `Are you sure you want to delete this ${config.title.toLowerCase()} record? This action cannot be undone.`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      isDestructive: true,
      onConfirm: () => {
        this.serviceInstance[config.deleteMethod!](record[config.idKey]).subscribe({
          next: () => { this.loadRecords(); this.toast.success('Record deleted'); },
          error: (err: any) => this.toast.error('Delete failed', err?.error)
        });
      }
    });
  }

  renderValue(record: any, key: string | number | symbol, column: ResourcePageConfig<any, any>['columns'][number]): string {
    const value = record?.[key as string];
    if (column.format) {
      return column.format(value, record);
    }

    if (value === null || value === undefined) {
      return '-';
    }

    if (column.type === 'date') {
      return new Date(value as string).toLocaleDateString();
    }

    return String(value);
  }

  renderBoolean(record: any, key: string | number | symbol): boolean {
    return Boolean(record?.[key as string]);
  }

  isControlInvalid(controlName: string): boolean {
    const control = this.form.get(controlName);
    return !!control && control.touched && control.invalid;
  }
}
