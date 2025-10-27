import { useCallback, useEffect, useRef } from 'react';
import { useAuthStore } from '../stores/auth-store';
import { refreshSession, logout as logoutRequest } from '../features/auth/api';

const SILENT_REFRESH_WINDOW_MS = 60_000;
const AUTH_STORAGE_KEY = 'auth.session';
let sessionRestorePromise: Promise<boolean> | null = null;

export function useAuth() {
  const user = useAuthStore((state) => state.user);
  const token = useAuthStore((state) => state.token);
  const hydrated = useAuthStore((state) => state.hydrated);
  const loginStore = useAuthStore((state) => state.login);
  const logoutStore = useAuthStore((state) => state.logout);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const setHydrated = useAuthStore((state) => state.setHydrated);
  const refreshTimerRef = useRef<number>();

  const logout = useCallback(async () => {
    try {
      await logoutRequest();
    } catch {
      // Sunucuya ulaşılamasa bile istemci oturumunu kapat
    } finally {
      logoutStore();
      if (typeof window !== 'undefined') {
        const attributes = ['Max-Age=0', 'Path=/'];
        const domain = window.location.hostname;
        if (domain && domain.includes('.')) {
          attributes.push(`Domain=${domain}`);
        }

        document.cookie = `${encodeURIComponent('blogapp_refresh_token')}=; ${attributes.join('; ')};`;
      }
    }
  }, [logoutStore]);

  useEffect(() => {
    if (hydrated || typeof window === 'undefined') {
      return;
    }

    const raw = window.sessionStorage.getItem(AUTH_STORAGE_KEY);
    if (!raw) {
      setHydrated(true);
      return;
    }

    try {
      const stored = JSON.parse(raw) as { user: typeof user; token: string };
      const expiresAt = new Date(stored.user?.expiration ?? '').getTime();
      if (Number.isNaN(expiresAt) || expiresAt <= Date.now()) {
        window.sessionStorage.removeItem(AUTH_STORAGE_KEY);
        setHydrated(true);
        return;
      }

      if (stored.user && stored.token) {
        loginStore({ user: stored.user, token: stored.token });
        return;
      }

      setHydrated(true);
    } catch {
      window.sessionStorage.removeItem(AUTH_STORAGE_KEY);
      setHydrated(true);
    }
  }, [hydrated, loginStore, setHydrated]);

  useEffect(() => {
    if (!hydrated || typeof window === 'undefined') {
      return;
    }

    if (token && user) {
      window.sessionStorage.setItem(
        AUTH_STORAGE_KEY,
        JSON.stringify({
          user,
          token
        })
      );
      return;
    }

    window.sessionStorage.removeItem(AUTH_STORAGE_KEY);
  }, [hydrated, token, user]);

  useEffect(() => {
    if (refreshTimerRef.current) {
      window.clearTimeout(refreshTimerRef.current);
      refreshTimerRef.current = undefined;
    }

    if (!hydrated || !token || !user) {
      return undefined;
    }

    const expiresAt = new Date(user.expiration).getTime();
    if (Number.isNaN(expiresAt)) {
      return undefined;
    }

    const delay = Math.max(expiresAt - Date.now() - SILENT_REFRESH_WINDOW_MS, 0);

    refreshTimerRef.current = window.setTimeout(async () => {
      try {
        const response = await refreshSession();
        if (response.success && response.data) {
          loginStore({
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
        await logout();
      } catch {
        await logout();
      }
    }, delay);

    return () => {
      if (refreshTimerRef.current) {
        window.clearTimeout(refreshTimerRef.current);
        refreshTimerRef.current = undefined;
      }
    };
  }, [hydrated, token, user, loginStore, logout]);

  const ensureSession = useCallback(async () => {
    const state = useAuthStore.getState();
    if (state.token && state.user) {
      return true;
    }

    if (sessionRestorePromise) {
      return sessionRestorePromise;
    }

    sessionRestorePromise = (async () => {
      try {
        const response = await refreshSession();
        if (response.success && response.data) {
          loginStore({
            user: {
              userId: response.data.userId,
              userName: response.data.userName,
              expiration: response.data.expiration,
              permissions: response.data.permissions
            },
            token: response.data.token
          });
          return true;
        }
        await logout();
        return false;
      } catch {
        await logout();
        return false;
      } finally {
        sessionRestorePromise = null;
      }
    })();

    return sessionRestorePromise;
  }, [loginStore, logout]);

  return {
    user,
    token,
    hydrated,
    login: loginStore,
    logout,
    isAuthenticated,
    ensureSession
  };
}
