import { PaginatedListResponse } from '../../types/api';

export interface Category {
  id: string;
  name: string;
  createdDate: string;
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
