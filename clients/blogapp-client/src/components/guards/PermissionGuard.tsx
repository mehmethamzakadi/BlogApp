import { ReactNode } from 'react';
import { usePermission } from '../../hooks/use-permission';
import { Navigate } from 'react-router-dom';
import toast from 'react-hot-toast';

interface PermissionGuardProps {
  /** 
   * Gerekli olan permission (tek bir permission için)
   */
  permission?: string;
  
  /**
   * Gerekli olan permission'lar (birden fazla permission için)
   * Varsayılan olarak tümü gereklidir (requireAll: true)
   */
  permissions?: string[];
  
  /**
   * permissions array kullanıldığında:
   * - true: Tüm permission'lar gereklidir (AND)
   * - false: En az bir permission yeterlidir (OR)
   * @default true
   */
  requireAll?: boolean;
  
  /**
   * Yetkisiz kullanıcı için gösterilecek içerik
   * Varsayılan olarak 403 sayfasına yönlendirir
   */
  fallback?: ReactNode;
  
  /**
   * Yetki yoksa toast göster
   * @default true
   */
  showToast?: boolean;
  
  /**
   * Korunacak içerik
   */
  children: ReactNode;
}

/**
 * Permission bazlı koruma componenti
 * 
 * @example
 * // Tek permission kontrolü
 * <PermissionGuard permission="Users.Create">
 *   <CreateUserButton />
 * </PermissionGuard>
 * 
 * @example
 * // Birden fazla permission kontrolü (tümü gerekli)
 * <PermissionGuard permissions={["Users.Create", "Users.Update"]}>
 *   <UserManagementPanel />
 * </PermissionGuard>
 * 
 * @example
 * // Birden fazla permission kontrolü (en az biri yeterli)
 * <PermissionGuard permissions={["Users.Read", "Users.ViewAll"]} requireAll={false}>
 *   <UsersList />
 * </PermissionGuard>
 * 
 * @example
 * // Özel fallback içerik
 * <PermissionGuard 
 *   permission="Posts.Delete"
 *   fallback={<div>Bu işlem için yetkiniz yok</div>}
 * >
 *   <DeleteButton />
 * </PermissionGuard>
 */
export function PermissionGuard({
  permission,
  permissions,
  requireAll = true,
  fallback,
  showToast = true,
  children
}: PermissionGuardProps) {
  const { hasPermission, hasAnyPermission, hasAllPermissions } = usePermission();

  let hasRequiredPermission = false;

  if (permission) {
    hasRequiredPermission = hasPermission(permission);
  } else if (permissions && permissions.length > 0) {
    hasRequiredPermission = requireAll 
      ? hasAllPermissions(...permissions)
      : hasAnyPermission(...permissions);
  } else {
    // Ne permission ne de permissions verilmişse, içeriği göster
    return <>{children}</>;
  }

  if (!hasRequiredPermission) {
    if (showToast) {
      const permissionText = permission || permissions?.join(', ') || 'bilinmeyen';
      toast.error(`Bu işlem için "${permissionText}" yetkisine sahip olmalısınız.`, {
        duration: 4000,
        icon: '🔒',
      });
    }

    if (fallback !== undefined) {
      return <>{fallback}</>;
    }

    // Varsayılan olarak 403 sayfasına yönlendir
    return <Navigate to="/forbidden" replace />;
  }

  return <>{children}</>;
}
