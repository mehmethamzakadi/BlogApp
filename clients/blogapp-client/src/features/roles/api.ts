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

export async function createRole(data: RoleFormValues): Promise<void> {
  await api.post('/role', data);
}

export async function updateRole(data: RoleUpdateFormValues): Promise<void> {
  await api.put(`/role/${data.id}`, data);
}

export async function deleteRole(id: string): Promise<void> {
  await api.delete(`/role/${id}`);
}
