import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';

export interface AuthUser {
  userId: string;
  userName: string;
  expiration: string;
  refreshToken: string;
  permissions: string[];
}

interface AuthState {
  user: AuthUser | null;
  token: string | null;
  hydrated: boolean;
  login: (payload: { user: AuthUser; token: string }) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
  setHydrated: (hydrated: boolean) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      hydrated: typeof window === 'undefined',
      login: ({ user, token }) => {
        set({ user, token });
      },
      logout: () => {
        set({ user: null, token: null });
      },
      setHydrated: (hydrated) => {
        set({ hydrated });
      },
      isAuthenticated: () => {
        const state = get();
        if (!state.user || !state.token) {
          return false;
        }

        const expiresAt = new Date(state.user.expiration).getTime();
        if (Number.isNaN(expiresAt) || expiresAt <= Date.now()) {
          set({ user: null, token: null });
          return false;
        }

        return true;
      }
    }),
    {
      name: 'blogapp-auth',
      storage: createJSONStorage(() => localStorage),
      partialize: ({ user, token }) => ({ user, token }),
      onRehydrateStorage: () => (state) => {
        state?.setHydrated(true);
      }
    }
  )
);
