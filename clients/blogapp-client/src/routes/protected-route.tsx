import { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/use-auth';
import { usePermission } from '../hooks/use-permission';
import { ForbiddenPage } from '../pages/error/forbidden-page';

interface ProtectedRouteProps {
  children: React.ReactNode;
  /** Gereken permission (opsiyonel, verilmezse sadece login kontrolü yapar) */
  requiredPermission?: string;
  /** Gereken permission'lardan herhangi biri (opsiyonel) */
  requiredAnyPermissions?: string[];
  /** Gereken tüm permission'lar (opsiyonel) */
  requiredAllPermissions?: string[];
}

export function ProtectedRoute({
  children,
  requiredPermission,
  requiredAnyPermissions,
  requiredAllPermissions
}: ProtectedRouteProps) {
  const location = useLocation();
  const { isAuthenticated, hydrated, ensureSession } = useAuth();
  const { hasPermission, hasAnyPermission, hasAllPermissions } = usePermission();
  const [sessionChecked, setSessionChecked] = useState(false);

  useEffect(() => {
    let cancelled = false;

    if (!hydrated) {
      return () => {
        cancelled = true;
      };
    }

    if (isAuthenticated) {
      if (!sessionChecked) {
        setSessionChecked(true);
      }
      return () => {
        cancelled = true;
      };
    }

    if (sessionChecked) {
      return () => {
        cancelled = true;
      };
    }

    ensureSession().finally(() => {
      if (!cancelled) {
        setSessionChecked(true);
      }
    });

    return () => {
      cancelled = true;
    };
  }, [hydrated, isAuthenticated, ensureSession, sessionChecked]);

  if (!hydrated || (!isAuthenticated && !sessionChecked)) {
    return null;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Permission kontrolü
  if (requiredPermission && !hasPermission(requiredPermission)) {
    return <ForbiddenPage />;
  }

  if (requiredAnyPermissions && !hasAnyPermission(...requiredAnyPermissions)) {
    return <ForbiddenPage />;
  }

  if (requiredAllPermissions && !hasAllPermissions(...requiredAllPermissions)) {
    return <ForbiddenPage />;
  }

  return <>{children}</>;
}
