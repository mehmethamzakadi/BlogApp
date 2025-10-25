import { useAuth } from './use-auth';

/**
 * Permission kontrolü için hook
 * 
 * @example
 * const { hasPermission, hasAnyPermission, hasAllPermissions } = usePermission();
 * 
 * if (hasPermission('Users.Create')) {
 *   // Kullanıcı oluşturma butonunu göster
 * }
 */
export function usePermission() {
  const { user } = useAuth();
  const permissions = user?.permissions || [];

  /**
   * Kullanıcının belirli bir permission'ı olup olmadığını kontrol eder
   */
  const hasPermission = (permission: string): boolean => {
    return permissions.includes(permission);
  };

  /**
   * Kullanıcının verilen permission'lardan en az birine sahip olup olmadığını kontrol eder
   */
  const hasAnyPermission = (...requiredPermissions: string[]): boolean => {
    return requiredPermissions.some((permission) => permissions.includes(permission));
  };

  /**
   * Kullanıcının verilen tüm permission'lara sahip olup olmadığını kontrol eder
   */
  const hasAllPermissions = (...requiredPermissions: string[]): boolean => {
    return requiredPermissions.every((permission) => permissions.includes(permission));
  };

  return {
    permissions,
    hasPermission,
    hasAnyPermission,
    hasAllPermissions
  };
}
