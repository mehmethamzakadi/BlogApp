import api from '../../lib/axios';
import { ApiResult, normalizeApiResult, normalizePaginatedResponse } from '../../types/api';
import {
  Category,
  CategoryFormValues,
  CategoryListResponse,
  CategoryTableFilters
} from './types';

function buildDataGridPayload(filters: CategoryTableFilters) {
  const payload: Record<string, unknown> = {
    PaginatedRequest: {
      PageIndex: filters.pageIndex,
      PageSize: filters.pageSize
    }
  };

  const sort = filters.sort
    ? [
        {
          Field: filters.sort.field,
          Dir: filters.sort.dir
        }
      ]
    : undefined;

  const filter = filters.search
    ? {
        Field: 'Name',
        Operator: 'contains',
        Value: filters.search
      }
    : undefined;

  if (sort || filter) {
    payload.DynamicQuery = {
      ...(sort ? { Sort: sort } : {}),
      ...(filter ? { Filter: filter } : {})
    };
  }

  return payload;
}

export async function fetchCategories(
  filters: CategoryTableFilters
): Promise<CategoryListResponse> {
  const response = await api.post('/category/search', buildDataGridPayload(filters));
  return normalizePaginatedResponse<Category>(response.data);
}

export async function createCategory(values: CategoryFormValues) {
  const response = await api.post<ApiResult>('/category', {
    Name: values.name
  });
  return normalizeApiResult(response.data);
}

export async function updateCategory(id: number, values: CategoryFormValues) {
  const response = await api.put<ApiResult>(`/category/${id}`, {
    Id: id,
    Name: values.name
  });
  return normalizeApiResult(response.data);
}

export async function deleteCategory(id: number) {
  const response = await api.delete<ApiResult>(`/category/${id}`);
  return normalizeApiResult(response.data);
}

export async function getAllCategories(): Promise<Category[]> {
  const response = await api.get<Category[]>('/category');
  return response.data;
}
