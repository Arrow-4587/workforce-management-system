import { Component } from '@angular/core';
import { ResourcePageComponent } from '../../shared/components/resource-page.component';
import { RoleService } from '../../core/services/role.service';
import { ResourcePageConfig } from '../../core/models/resource-page.models';
import { CreateRole, RoleResponse, UpdateRole } from '../../core/models/wms.models';

interface RoleForm {
  roleName: string;
  description: string;
}

const roleConfig: ResourcePageConfig<RoleResponse, RoleForm> = {
  title: 'Roles',
  description: 'Define application roles and their descriptive labels.',
  searchPlaceholder: 'Search roles',
  service: RoleService,
  loadMethod: 'getAll',
  createMethod: 'create',
  updateMethod: 'update',
  deleteMethod: 'delete',
  idKey: 'roleId',
  columns: [
    { key: 'roleId', label: 'ID', type: 'number' },
    { key: 'roleName', label: 'Role' },
    { key: 'description', label: 'Description' }
  ],
  fields: [
    { name: 'roleName', label: 'Role name', type: 'text', required: true, maxLength: 50 },
    { name: 'description', label: 'Description', type: 'textarea', maxLength: 255 }
  ],
  emptyForm: () => ({ roleName: '', description: '' }),
  toForm: record => ({ roleName: record.roleName, description: record.description }),
  toCreatePayload: form => ({ roleName: form.roleName, description: form.description }),
  toUpdatePayload: form => ({ roleName: form.roleName, description: form.description })
};

@Component({
  standalone: true,
  imports: [ResourcePageComponent],
  templateUrl: './role.page.html'
})
export class RolePageComponent {
  protected readonly config = roleConfig;
}
