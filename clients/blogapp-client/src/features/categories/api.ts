import api from '../../lib/axios';
import { ApiResult, normalizeApiResult, normalizePaginatedResponse } from '../../types/api';
import { buildDataGridPayload } from '../../lib/data-grid-helpers';
import {
  Category,
  CategoryFormValues,
  CategoryListResponse,
  CategoryTableFilters
} from './types';

export async function fetchCategories(
  filters: CategoryTableFilters
): Promise<CategoryListResponse> {
  const response = await api.post('/category/search', buildDataGridPayload(filters, 'Name'));
  return normalizePaginatedResponse<Category>(response.data);
}

export async function createCategory(values: CategoryFormValues) {
  const response = await api.post<ApiResult>('/category', {
    Name: values.name
  });
  return normalizeApiResult(response.data);
}

export async function updateCategory(id: string, values: CategoryFormValues) {
  const response = await api.put<ApiResult>(`/category/${id}`, {
    Id: id,
    Name: values.name
  });
  return normalizeApiResult(response.data);
}

export async function deleteCategory(id: string) {
  const response = await api.delete<ApiResult>(`/category/${id}`);
  return normalizeApiResult(response.data);
}

export async function getAllCategories(): Promise<Category[]> {
  const response = await api.get<Category[]>('/category');
  return response.data;
}
