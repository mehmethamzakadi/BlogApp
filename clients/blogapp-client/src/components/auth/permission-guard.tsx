import { usePermission } from '../../hooks/use-permission';

interface PermissionGuardProps {
  children: React.ReactNode;
  /** Gereken permission */
  requiredPermission?: string;
  /** Gereken permission'lardan herhangi biri */
  requiredAnyPermissions?: string[];
  /** Gereken tüm permission'lar */
  requiredAllPermissions?: string[];
  /** Yetkisi yoksa gösterilecek fallback (varsayılan: null) */
  fallback?: React.ReactNode;
}

/**
 * Permission bazlı koşullu render component'i
 * Kullanıcının gerekli permission'ı yoksa children'ı göstermez
 * 
 * @example
 * <PermissionGuard requiredPermission={Permissions.UsersCreate}>
 *   <Button>Kullanıcı Oluştur</Button>
 * </PermissionGuard>
 */
export function PermissionGuard({
  children,
  requiredPermission,
  requiredAnyPermissions,
  requiredAllPermissions,
  fallback = null
}: PermissionGuardProps) {
  const { hasPermission, hasAnyPermission, hasAllPermissions } = usePermission();

  let hasAccess = true;

  if (requiredPermission) {
    hasAccess = hasPermission(requiredPermission);
  } else if (requiredAnyPermissions) {
    hasAccess = hasAnyPermission(...requiredAnyPermissions);
  } else if (requiredAllPermissions) {
    hasAccess = hasAllPermissions(...requiredAllPermissions);
  }

  if (!hasAccess) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}
