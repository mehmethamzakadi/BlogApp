import api from '../../lib/axios';
import { ApiResult, normalizeApiResult, normalizePaginatedResponse } from '../../types/api';
import { buildMultiFieldDataGridPayload } from '../../lib/data-grid-helpers';
import {
  User,
  UserFormValues,
  UserUpdateFormValues,
  AssignRolesFormValues,
  UserListResponse,
  UserTableFilters,
  UserRolesResponse
} from './types';

export async function fetchUsers(filters: UserTableFilters): Promise<UserListResponse> {
  const searchFields = ['FirstName', 'LastName', 'Email', 'UserName'];
  const response = await api.post('/user/search', buildMultiFieldDataGridPayload(filters, searchFields));
  return normalizePaginatedResponse<User>(response.data);
}

export async function fetchUserById(id: string): Promise<User> {
  const response = await api.get<ApiResult<User>>(`/user/${id}`);
  const result = normalizeApiResult<User>(response.data);
  return result.data;
}

export async function createUser(data: UserFormValues): Promise<void> {
  await api.post('/user', data);
}

export async function updateUser(data: UserUpdateFormValues): Promise<void> {
  await api.put(`/user/${data.id}`, data);
}

export async function deleteUser(id: string): Promise<void> {
  await api.delete(`/user/${id}`);
}

export async function fetchUserRoles(userId: string): Promise<UserRolesResponse> {
  const response = await api.get<ApiResult<UserRolesResponse>>(`/user/${userId}/roles`);
  const result = normalizeApiResult<UserRolesResponse>(response.data);
  return result.data;
}

export async function assignRolesToUser(data: AssignRolesFormValues): Promise<void> {
  await api.post(`/user/${data.userId}/roles`, data);
}
