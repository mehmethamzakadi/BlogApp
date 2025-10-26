export interface Permission {
  id: string;
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
  roleId: string;
  roleName: string;
  permissionIds: string[];
}

export interface AssignPermissionsFormValues {
  roleId: string;
  permissionIds: string[];
}
