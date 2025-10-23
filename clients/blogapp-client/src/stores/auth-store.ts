import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';

export interface AuthUser {
  userId: number;
  userName: string;
  expiration: string;
  refreshToken: string;
}

interface AuthState {
  user: AuthUser | null;
  token: string | null;
  login: (payload: { user: AuthUser; token: string }) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      login: ({ user, token }) => {
        set({ user, token });
      },
      logout: () => {
        set({ user: null, token: null });
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
      storage: createJSONStorage(() => localStorage)
    }
  )
);
