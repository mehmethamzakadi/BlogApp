import { PaginatedListResponse } from '../../types/api';

export interface Role {
  id: number;
  name: string;
  normalizedName?: string;
  concurrencyStamp?: string;
}

export type RoleListResponse = PaginatedListResponse<Role>;

export interface RoleFormValues {
  name: string;
}

export interface RoleUpdateFormValues {
  id: number;
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
