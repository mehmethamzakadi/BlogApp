import { QueryClient } from '@tanstack/react-query';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      // Data 5 saniye sonra stale olur (güncel olmadığını kabul eder)
      staleTime: 5 * 1000, // 5 saniye (1 dakikadan çok daha kısa)
      // Cache 5 dakika boyunca tutulur
      gcTime: 5 * 60 * 1000, // 5 dakika
      // Pencere focus olduğunda otomatik refetch YOK (manuel invalidation kullanıyoruz)
      refetchOnWindowFocus: false,
      // Mount olduğunda stale data'yı refetch et
      refetchOnMount: true,
      // Network yeniden bağlandığında refetch YOK
      refetchOnReconnect: false
    }
  }
});
