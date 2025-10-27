import { useCallback, useEffect, useRef } from 'react';
import { useAuthStore } from '../stores/auth-store';
import { logout as logoutRequest } from '../features/auth/api';
import { refreshAccessToken } from '../lib/axios';

const SILENT_REFRESH_WINDOW_MS = 60_000;
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

  const tryRestoreSession = useCallback(async (): Promise<boolean> => {
    try {
      await refreshAccessToken();
      return true;
    } catch (error) {
      await logout();
      return false;
    }
  }, [logout]);

  const ensureSession = useCallback(async () => {
    const state = useAuthStore.getState();
    if (state.token && state.user) {
      if (!state.hydrated) {
        setHydrated(true);
      }
      return true;
    }

    if (sessionRestorePromise) {
      return sessionRestorePromise;
    }

    sessionRestorePromise = (async () => {
      try {
        return await tryRestoreSession();
      } finally {
        sessionRestorePromise = null;
      }
    })();

    return sessionRestorePromise;
  }, [setHydrated, tryRestoreSession]);

  useEffect(() => {
    if (hydrated || typeof window === 'undefined') {
      return;
    }

    let cancelled = false;

    (async () => {
      try {
        await ensureSession();
      } finally {
        if (!cancelled) {
          const currentState = useAuthStore.getState();
          if (!currentState.hydrated) {
            setHydrated(true);
          }
        }
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [ensureSession, hydrated, setHydrated]);

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
        await tryRestoreSession();
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
  }, [hydrated, token, user, logout, tryRestoreSession]);

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
