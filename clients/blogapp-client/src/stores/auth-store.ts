import { create } from 'zustand';
import type { AuthUser } from '../types/auth';

interface AuthState {
  user: AuthUser | null;
  token: string | null;
  hydrated: boolean;
  login: (payload: { user: AuthUser; token: string }) => void;
  logout: () => void;
  isAuthenticated: boolean;
  setHydrated: (hydrated: boolean) => void;
}

const isTokenValid = (authUser: AuthUser | null) => {
  if (!authUser) {
    return false;
  }

  const expiresAt = new Date(authUser.expiration).getTime();
  return !Number.isNaN(expiresAt) && expiresAt > Date.now();
};

export const useAuthStore = create<AuthState>()((set) => ({
  user: null,
  token: null,
  hydrated: false,
  isAuthenticated: false,
  login: ({ user, token }) => {
    if (!isTokenValid(user)) {
      set({ user: null, token: null, isAuthenticated: false, hydrated: true });
      return;
    }

    set({ user, token, isAuthenticated: true, hydrated: true });
  },
  logout: () => {
    set({ user: null, token: null, isAuthenticated: false, hydrated: true });
  },
  setHydrated: (hydrated) => {
    set({ hydrated });
  }
}));
