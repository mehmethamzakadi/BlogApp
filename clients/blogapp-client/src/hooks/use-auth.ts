import { useEffect } from 'react';
import { useAuthStore } from '../stores/auth-store';
import { refreshSession } from '../features/auth/api';

let hydrationPromise: Promise<void> | null = null;

export function useAuth() {
  const user = useAuthStore((state) => state.user);
  const token = useAuthStore((state) => state.token);
  const hydrated = useAuthStore((state) => state.hydrated);
  const login = useAuthStore((state) => state.login);
  const logout = useAuthStore((state) => state.logout);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const setHydrated = useAuthStore((state) => state.setHydrated);

  useEffect(() => {
    if (hydrated) {
      return;
    }

    if (!hydrationPromise) {
      hydrationPromise = (async () => {
        try {
          const response = await refreshSession();

          if (response.success && response.data) {
            login({
              user: {
                userId: response.data.userId,
                userName: response.data.userName,
                expiration: response.data.expiration,
                permissions: response.data.permissions
              },
              token: response.data.token
            });
            return;
          }

          logout();
        } catch {
          logout();
        } finally {
          setHydrated(true);
          hydrationPromise = null;
        }
      })();
    }
  }, [hydrated, login, logout, setHydrated]);

  return {
    user,
    token,
    hydrated,
    login,
    logout,
    isAuthenticated
  };
}
