import { PaginatedListResponse } from '../../types/api';

export interface Category {
  id: string;
  name: string;
  description?: string;
  parentId?: string;
  parentName?: string;
  createdDate: string;
}

export type CategoryListResponse = PaginatedListResponse<Category>;

export interface CategoryFormValues {
  name: string;
  description?: string;
  parentId?: string;
}

export interface CategoryTableFilters {
  search?: string;
  pageIndex: number;
  pageSize: number;
  sort?: {
    field: string;
    dir: 'asc' | 'desc';
  };
}
