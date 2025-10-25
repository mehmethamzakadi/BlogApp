import axios, { AxiosHeaders, InternalAxiosRequestConfig } from 'axios';
import { normalizeApiError } from '../types/api';
import { useAuthStore } from '../stores/auth-store';
import toast from 'react-hot-toast';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  withCredentials: true
});

// Token yenilenirken aynı anda birden fazla istek yapılmasını önlemek için
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value?: unknown) => void;
  reject: (reason?: unknown) => void;
}> = [];

const processQueue = (error: Error | null, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  
  // Refresh token endpoint'i için Authorization header ekleme
  if (config.url?.includes('/Auth/RefreshToken')) {
    return config;
  }
  
  if (token) {
    const headers = AxiosHeaders.from(config.headers ?? {});
    headers.set('Authorization', `Bearer ${token}`);
    config.headers = headers;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

    // 403 Forbidden - Yetki hatası
    if (error.response?.status === 403) {
      toast.error('Bu işlem için yetkiniz bulunmamaktadır.', {
        duration: 4000,
        icon: '🔒',
      });
      return Promise.reject(normalizeApiError(error, 'Bu işlem için yetkiniz bulunmamaktadır.'));
    }

    // Login, register gibi authentication endpoint'leri için 401 hatasını ignore et
    const authEndpoints = ['/auth/login', '/auth/register', '/Auth/Login', '/Auth/Register'];
    const isAuthEndpoint = authEndpoints.some(endpoint => originalRequest.url?.toLowerCase().includes(endpoint.toLowerCase()));

    // 401 hatası ve henüz retry edilmemişse ve auth endpoint değilse
    if (error.response?.status === 401 && !originalRequest._retry && !isAuthEndpoint) {
      if (isRefreshing) {
        // Zaten token yenileniyor, sıraya ekle
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            const headers = AxiosHeaders.from(originalRequest.headers ?? {});
            headers.set('Authorization', `Bearer ${token}`);
            originalRequest.headers = headers;
            return api(originalRequest);
          })
          .catch((err) => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const { user, logout } = useAuthStore.getState();

      if (!user?.refreshToken) {
        // Refresh token yoksa çıkış yap
        logout();
        if (typeof window !== 'undefined') {
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }

      try {
        // Refresh token API çağrısı - döngüsel bağımlılığı önlemek için doğrudan axios kullan
        const response = await axios.post(
          `${import.meta.env.VITE_API_URL}/Auth/RefreshToken`,
          { refreshToken: user.refreshToken },
          { withCredentials: true }
        );

        const newToken = response.data.data.token;
        const newUser = {
          userId: response.data.data.userId,
          userName: response.data.data.userName,
          expiration: response.data.data.expiration,
          refreshToken: response.data.data.refreshToken,
          permissions: response.data.data.permissions || []
        };

        // Store'u güncelle
        useAuthStore.getState().login({ user: newUser, token: newToken });

        // Başarılı olan istekleri işle
        processQueue(null, newToken);

        // Orijinal isteği yeni token ile tekrar dene
        const headers = AxiosHeaders.from(originalRequest.headers ?? {});
        headers.set('Authorization', `Bearer ${newToken}`);
        originalRequest.headers = headers;

        return api(originalRequest);
      } catch (refreshError) {
        // Refresh token başarısız, çıkış yap
        processQueue(refreshError as Error, null);
        logout();
        if (typeof window !== 'undefined') {
          window.location.href = '/login';
        }
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    const normalizedError = normalizeApiError(error, 'İsteğiniz işlenirken bir hata oluştu.');
    return Promise.reject(normalizedError);
  }
);

export default api;
