import { Component } from '@angular/core';
import { ResourcePageComponent } from '../../shared/components/resource-page.component';
import { DepartmentService } from '../../core/services/department.service';
import { ResourcePageConfig } from '../../core/models/resource-page.models';
import { DepartmentResponse, CreateDepartment, UpdateDepartment } from '../../core/models/wms.models';

interface DepartmentForm {
  departmentName: string;
  description: string;
}

const departmentConfig: ResourcePageConfig<DepartmentResponse, DepartmentForm> = {
  title: 'Departments',
  description: 'Manage organizational departments, descriptions, and staffing structure.',
  searchPlaceholder: 'Search departments',
  service: DepartmentService,
  loadMethod: 'getAll',
  searchMethod: 'search',
  createMethod: 'create',
  updateMethod: 'update',
  deleteMethod: 'delete',
  idKey: 'departmentId',
  columns: [
    { key: 'departmentId', label: 'ID', type: 'number' },
    { key: 'departmentName', label: 'Department' },
    { key: 'description', label: 'Description' },
    { key: 'createdOn', label: 'Created On', type: 'date' }
  ],
  fields: [
    { name: 'departmentName', label: 'Department name', type: 'text', required: true, maxLength: 100 },
    { name: 'description', label: 'Description', type: 'textarea', maxLength: 255 }
  ],
  emptyForm: () => ({ departmentName: '', description: '' }),
  toForm: record => ({ departmentName: record.departmentName, description: record.description ?? '' }),
  toCreatePayload: form => ({ departmentName: form.departmentName, description: form.description || undefined }),
  toUpdatePayload: form => ({ departmentName: form.departmentName, description: form.description || undefined })
};

@Component({
  standalone: true,
  imports: [ResourcePageComponent],
  templateUrl: './department.page.html'
})
export class DepartmentPageComponent {
  protected readonly config = departmentConfig;
}
