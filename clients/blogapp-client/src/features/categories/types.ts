import { PaginatedListResponse } from '../../types/api';

export interface Category {
  id: number;
  name: string;
}

export type CategoryListResponse = PaginatedListResponse<Category>;

export interface CategoryFormValues {
  name: string;
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
