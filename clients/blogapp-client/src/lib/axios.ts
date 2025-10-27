import axios, { AxiosHeaders, InternalAxiosRequestConfig } from 'axios';
import { normalizeApiError } from '../types/api';
import { useAuthStore } from '../stores/auth-store';
import toast from 'react-hot-toast';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  withCredentials: true
});

const REFRESH_THRESHOLD_MS = 60_000;

type QueueEntry = {
  resolve: (value: string) => void;
  reject: (reason?: unknown) => void;
};

let isRefreshing = false;
let failedQueue: QueueEntry[] = [];

const processQueue = (error: Error | null, token?: string) => {
  failedQueue.forEach((entry) => {
    if (error) {
      entry.reject(error);
    } else if (token) {
      entry.resolve(token);
    }
  });

  failedQueue = [];
};

export const refreshAccessToken = async (): Promise<string> => {
  if (isRefreshing) {
    return new Promise<string>((resolve, reject) => {
      failedQueue.push({ resolve, reject });
    });
  }

  isRefreshing = true;

  try {
    const response = await axios.post(
      `${import.meta.env.VITE_API_URL}/auth/refresh-token`,
      {},
      { withCredentials: true }
    );

    const payload = response.data?.data;
    if (!payload?.token) {
      throw new Error('Geçersiz refresh yanıtı alındı.');
    }

    const newToken = payload.token as string;
    const newUser = {
      userId: payload.userId,
      userName: payload.userName,
      expiration: payload.expiration,
      permissions: payload.permissions || []
    };

    useAuthStore.getState().login({ user: newUser, token: newToken });

    processQueue(null, newToken);
    return newToken;
  } catch (err) {
    const error = err instanceof Error ? err : new Error('Refresh token talebi başarısız oldu.');
    processQueue(error);
    useAuthStore.getState().logout();
    throw error;
  } finally {
    isRefreshing = false;
  }
};

api.interceptors.request.use(async (config) => {
  const requestUrl = config.url?.toLowerCase() ?? '';
  if (requestUrl.includes('/auth/refresh-token')) {
    return config;
  }

  const { token, user } = useAuthStore.getState();
  let authToken = token ?? undefined;

  if (token && user) {
    const expiresAt = new Date(user.expiration).getTime();
    if (!Number.isNaN(expiresAt) && expiresAt - Date.now() <= REFRESH_THRESHOLD_MS) {
      try {
        authToken = await refreshAccessToken();
      } catch {
        authToken = undefined;
      }
    }
  }

  if (authToken) {
    const headers = AxiosHeaders.from(config.headers ?? {});
    headers.set('Authorization', `Bearer ${authToken}`);
    config.headers = headers;
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };
    const requestUrl = originalRequest.url?.toLowerCase() ?? '';

    // 403 Forbidden - Yetki hatası
    if (error.response?.status === 403) {
      toast.error('Bu işlem için yetkiniz bulunmamaktadır.', {
        duration: 4000,
        icon: '🔒',
      });
      return Promise.reject(normalizeApiError(error, 'Bu işlem için yetkiniz bulunmamaktadır.'));
    }

    if (error.response?.status === 401 && requestUrl.includes('/auth/refresh-token')) {
      const { logout } = useAuthStore.getState();
      logout();
      return Promise.reject(error);
    }

    // Login, register gibi authentication endpoint'leri için 401 hatasını ignore et
    const authEndpoints = ['/auth/login', '/auth/register', '/Auth/Login', '/Auth/Register'];
    const isAuthEndpoint = authEndpoints.some((endpoint) => requestUrl.includes(endpoint.toLowerCase()));

    // 401 hatası ve henüz retry edilmemişse ve auth endpoint değilse
    if (error.response?.status === 401 && !originalRequest._retry && !isAuthEndpoint) {
      originalRequest._retry = true;
      try {
        const newToken = await refreshAccessToken();
        const headers = AxiosHeaders.from(originalRequest.headers ?? {});
        headers.set('Authorization', `Bearer ${newToken}`);
        originalRequest.headers = headers;
        return api(originalRequest);
      } catch (refreshError) {
        return Promise.reject(refreshError);
      }
    }

    const normalizedError = normalizeApiError(error, 'İsteğiniz işlenirken bir hata oluştu.');
    return Promise.reject(normalizedError);
  }
);

export default api;
