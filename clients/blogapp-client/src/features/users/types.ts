import { PaginatedListResponse } from '../../types/api';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  createdDate: string;
  isDeleted: boolean;
}

export interface UserRole {
  id: string;
  name: string;
}

export interface UserRolesResponse {
  userId: string;
  userName: string;
  email: string;
  roles: UserRole[];
}

export type UserListResponse = PaginatedListResponse<User>;

export interface UserFormValues {
  userName: string;
  email: string;
  password: string;
}

export interface UserUpdateFormValues {
  id: string;
  userName: string;
  email: string;
}

export interface AssignRolesFormValues {
  userId: string;
  roleIds: string[];
}

export interface UserTableFilters {
  search?: string;
  pageIndex: number;
  pageSize: number;
  sort?: {
    field: string;
    dir: 'asc' | 'desc';
  };
}
