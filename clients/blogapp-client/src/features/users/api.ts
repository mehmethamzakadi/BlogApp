import api from '../../lib/axios';
import { ApiResult, normalizeApiResult, normalizePaginatedResponse } from '../../types/api';
import {
  User,
  UserFormValues,
  UserUpdateFormValues,
  AssignRolesFormValues,
  UserListResponse,
  UserTableFilters,
  UserRolesResponse
} from './types';

function buildDataGridPayload(filters: UserTableFilters) {
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
        Logic: 'or',
        Filters: [
          { Field: 'FirstName', Operator: 'contains', Value: filters.search },
          { Field: 'LastName', Operator: 'contains', Value: filters.search },
          { Field: 'Email', Operator: 'contains', Value: filters.search },
          { Field: 'UserName', Operator: 'contains', Value: filters.search }
        ]
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

export async function fetchUsers(filters: UserTableFilters): Promise<UserListResponse> {
  const response = await api.post('/user/search', buildDataGridPayload(filters));
  return normalizePaginatedResponse<User>(response.data);
}

export async function fetchUserById(id: number): Promise<User> {
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

export async function deleteUser(id: number): Promise<void> {
  await api.delete(`/user/${id}`);
}

export async function fetchUserRoles(userId: number): Promise<UserRolesResponse> {
  const response = await api.get<ApiResult<UserRolesResponse>>(`/user/${userId}/roles`);
  const result = normalizeApiResult<UserRolesResponse>(response.data);
  return result.data;
}

export async function assignRolesToUser(data: AssignRolesFormValues): Promise<void> {
  await api.post(`/user/${data.userId}/roles`, data);
}
