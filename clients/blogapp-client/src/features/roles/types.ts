import { PaginatedListResponse } from '../../types/api';

export interface Role {
  id: string;
  name: string;
  normalizedName?: string;
  concurrencyStamp?: string;
  createdDate: string;
}

export type RoleListResponse = PaginatedListResponse<Role>;

export interface RoleFormValues {
  name: string;
}

export interface RoleUpdateFormValues {
  id: string;
  name: string;
}

export interface RoleTableFilters {
  search?: string;
  pageIndex: number;
  pageSize: number;
  sort?: {
    field: string;
    dir: 'asc' | 'desc';
  };
}
