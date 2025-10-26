import { useEffect, useRef } from 'react';
import { useAuthStore } from '../stores/auth-store';
import { refreshSession } from '../features/auth/api';

export function useAuth() {
  const user = useAuthStore((state) => state.user);
  const token = useAuthStore((state) => state.token);
  const hydrated = useAuthStore((state) => state.hydrated);
  const login = useAuthStore((state) => state.login);
  const logout = useAuthStore((state) => state.logout);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const setHydrated = useAuthStore((state) => state.setHydrated);
  const hasInitialized = useRef(false);

  useEffect(() => {
    if (hydrated || hasInitialized.current) {
      return;
    }

    hasInitialized.current = true;

    const hydrateFromSession = async () => {
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
      }
    };

    void hydrateFromSession();
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
