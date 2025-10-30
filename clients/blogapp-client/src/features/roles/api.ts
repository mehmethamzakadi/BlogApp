import api from '../../lib/axios';
import { ApiResult, normalizeApiResult, normalizePaginatedResponse } from '../../types/api';
import { Role, RoleFormValues, RoleUpdateFormValues, RoleListResponse } from './types';

export async function fetchRoles(
  pageIndex: number = 0,
  pageSize: number = 10
): Promise<RoleListResponse> {
  const response = await api.get('/role', {
    params: { PageIndex: pageIndex, PageSize: pageSize }
  });
  return normalizePaginatedResponse<Role>(response.data);
}

export async function fetchRoleById(id: string): Promise<Role> {
  const response = await api.get<ApiResult<Role>>(`/role/${id}`);
  const result = normalizeApiResult<Role>(response.data);
  return result.data;
}

export async function createRole(data: RoleFormValues): Promise<ApiResult> {
  const response = await api.post('/role', data);
  return normalizeApiResult(response.data);
}

export async function updateRole(data: RoleUpdateFormValues): Promise<ApiResult> {
  const response = await api.put(`/role/${data.id}`, data);
  return normalizeApiResult(response.data);
}

export async function deleteRole(id: string): Promise<ApiResult> {
  const response = await api.delete(`/role/${id}`);
  return normalizeApiResult(response.data);
}
