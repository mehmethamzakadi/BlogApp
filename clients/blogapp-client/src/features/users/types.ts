import { PaginatedListResponse } from '../../types/api';

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  createdDate: string;
  isDeleted: boolean;
}

export interface UserRole {
  id: number;
  name: string;
}

export interface UserRolesResponse {
  userId: number;
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
  id: number;
  userName: string;
  email: string;
}

export interface AssignRolesFormValues {
  userId: number;
  roleIds: number[];
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
