import api from '../../lib/axios';
import { ApiResult, normalizeApiResult } from '../../types/api';
import {
  AllPermissionsResponse,
  RolePermissionsResponse,
  AssignPermissionsFormValues
} from './types';

export async function fetchAllPermissions(): Promise<AllPermissionsResponse> {
  const response = await api.get<ApiResult<AllPermissionsResponse>>('/permission');
  const result = normalizeApiResult<AllPermissionsResponse>(response.data);
  return result.data;
}

export async function fetchRolePermissions(roleId: string): Promise<RolePermissionsResponse> {
  const response = await api.get<ApiResult<RolePermissionsResponse>>(`/permission/role/${roleId}`);
  const result = normalizeApiResult<RolePermissionsResponse>(response.data);
  return result.data;
}

export async function assignPermissionsToRole(data: AssignPermissionsFormValues): Promise<ApiResult> {
  const response = await api.post(`/permission/role/${data.roleId}`, data);
  return normalizeApiResult(response.data);
}
