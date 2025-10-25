export interface Permission {
  id: number;
  name: string;
  description: string;
  type: string;
}

export interface PermissionModule {
  moduleName: string;
  permissions: Permission[];
}

export interface AllPermissionsResponse {
  modules: PermissionModule[];
}

export interface RolePermissionsResponse {
  roleId: number;
  roleName: string;
  permissionIds: number[];
}

export interface AssignPermissionsFormValues {
  roleId: number;
  permissionIds: number[];
}
