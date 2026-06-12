import { Component } from '@angular/core';
import { ResourcePageComponent } from '../../shared/components/resource-page.component';
import { ClientService } from '../../core/services/client.service';
import { ResourcePageConfig } from '../../core/models/resource-page.models';
import { ClientResponse, CreateClient, UpdateClient } from '../../core/models/wms.models';

interface ClientForm {
  clientName: string;
  clientAddress: string;
  clientPhoneNumber: string;
  clientLocation: string;
  status: boolean;
}

const clientConfig: ResourcePageConfig<ClientResponse, ClientForm> = {
  title: 'Clients',
  description: 'Track client accounts, locations, and active status.',
  searchPlaceholder: 'Search clients',
  service: ClientService,
  loadMethod: 'getAll',
  createMethod: 'create',
  updateMethod: 'update',
  deleteMethod: 'delete',
  idKey: 'clientId',
  columns: [
    { key: 'clientId', label: 'ID', type: 'number' },
    { key: 'clientName', label: 'Client' },
    { key: 'clientLocation', label: 'Location' },
    { key: 'clientPhoneNumber', label: 'Phone' },
    { key: 'status', label: 'Status', type: 'boolean' }
  ],
  fields: [
    { name: 'clientName', label: 'Client name', type: 'text', required: true },
    { name: 'clientAddress', label: 'Address', type: 'textarea' },
    { name: 'clientPhoneNumber', label: 'Phone number', type: 'text', pattern: /^[0-9]{10,15}$/, patternErrorMessage: 'Phone number must contain 10-15 digits.' },
    { name: 'clientLocation', label: 'Location', type: 'text' },
    { name: 'status', label: 'Active', type: 'checkbox' }
  ],
  emptyForm: () => ({ clientName: '', clientAddress: '', clientPhoneNumber: '', clientLocation: '', status: true }),
  toForm: record => ({
    clientName: record.clientName,
    clientAddress: record.clientAddress ?? '',
    clientPhoneNumber: record.clientPhoneNumber ?? '',
    clientLocation: record.clientLocation ?? '',
    status: record.status
  }),
  toCreatePayload: form => ({
    clientName: form.clientName,
    clientAddress: form.clientAddress || undefined,
    clientPhoneNumber: form.clientPhoneNumber || undefined,
    clientLocation: form.clientLocation || undefined,
    status: form.status
  }),
  toUpdatePayload: form => ({
    clientName: form.clientName,
    clientAddress: form.clientAddress || undefined,
    clientPhoneNumber: form.clientPhoneNumber || undefined,
    clientLocation: form.clientLocation || undefined,
    status: form.status
  })
};

@Component({
  standalone: true,
  imports: [ResourcePageComponent],
  templateUrl: './client.page.html'
})
export class ClientPageComponent {
  protected readonly config = clientConfig;
}
