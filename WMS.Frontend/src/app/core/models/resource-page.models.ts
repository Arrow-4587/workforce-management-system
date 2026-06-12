export type ResourceFieldType = 'text' | 'textarea' | 'number' | 'date' | 'email' | 'select' | 'checkbox';

export interface ResourceFieldOption {
  label: string;
  value: string | number | boolean;
}

export interface ResourceFieldConfig<TForm extends Record<string, any>> {
  name: keyof TForm & string;
  label: string;
  type: ResourceFieldType;
  placeholder?: string;
  required?: boolean;
  maxLength?: number;
  minLength?: number;
  pattern?: string | RegExp;
  patternErrorMessage?: string;
  options?: ResourceFieldOption[];
}

export interface ResourceColumnConfig<TEntity> {
  key: keyof TEntity | string;
  label: string;
  type?: 'text' | 'date' | 'number' | 'boolean';
  badgeClass?: (row: TEntity) => string;
  format?: (value: unknown, row: TEntity) => string;
}

export interface ResourcePageConfig<TEntity, TForm extends Record<string, any>> {
  title: string;
  description: string;
  searchPlaceholder?: string;
  service: new (...args: any[]) => any;
  loadMethod: string;
  searchMethod?: string;
  createMethod?: string;
  updateMethod?: string;
  deleteMethod?: string;
  idKey: keyof TEntity & string;
  columns: ResourceColumnConfig<TEntity>[];
  fields: ResourceFieldConfig<TForm>[];
  emptyForm: () => TForm;
  toForm?: (record: TEntity) => TForm;
  toCreatePayload?: (form: TForm) => unknown;
  toUpdatePayload?: (form: TForm) => unknown;
  searchable?: boolean;
  canCreate?: boolean;
  canEdit?: boolean;
  canDelete?: boolean;
  pageSize?: number;
}
