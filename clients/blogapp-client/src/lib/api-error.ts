import toast from 'react-hot-toast';
import { ApiError, ApiResult } from '../types/api';

function isApiError(error: unknown): error is ApiError {
  return Boolean(
    error &&
      typeof error === 'object' &&
      'success' in error &&
      'message' in error &&
      (error as ApiError).success === false
  );
}

export function getApiErrorMessage(error: unknown, fallbackMessage = 'Beklenmeyen bir hata oluştu'): string {
  if (!error) {
    return fallbackMessage;
  }

  if (typeof error === 'string') {
    return error;
  }

  if (isApiError(error)) {
    if (Array.isArray(error.errors) && error.errors.length > 0) {
      return error.errors.filter(Boolean).join('\n');
    }

    if (error.message) {
      return error.message;
    }
  }

  if (typeof error === 'object' && 'message' in error && typeof (error as { message?: unknown }).message === 'string') {
    return (error as { message?: string }).message || fallbackMessage;
  }

  if (error instanceof Error) {
    return error.message || fallbackMessage;
  }

  return fallbackMessage;
}

/**
 * API hatalarını toast ile gösterir. Eğer errors array varsa liste formatında gösterir.
 */
export function handleApiError(error: unknown, fallbackMessage = 'Beklenmeyen bir hata oluştu'): string {
  const message = getApiErrorMessage(error, fallbackMessage);
  
  // Eğer error objesi varsa ve errors array'i varsa, liste halinde göster
  if (error && typeof error === 'object' && 'errors' in error) {
    const apiError = error as ApiError;
    if (Array.isArray(apiError.errors) && apiError.errors.length > 0) {
      const errorList = apiError.errors.filter(Boolean);
      if (errorList.length > 0) {
        // Başlık + bullet liste formatı
        const formattedMessage = `${apiError.message || fallbackMessage}:\n${errorList.map(err => `• ${err}`).join('\n')}`;
        toast.error(formattedMessage, { 
          duration: 5000,
          style: { 
            whiteSpace: 'pre-line',
            maxWidth: '500px'
          }
        });
        return formattedMessage;
      }
    }
  }
  
  toast.error(message);
  return message;
}

/**
 * API response'daki hataları toast ile gösterir (onSuccess callback'lerde kullanılır).
 * Eğer errors array varsa liste formatında gösterir.
 */
export function showApiResponseError(response: ApiResult<any>, fallbackMessage = 'İşlem başarısız oldu'): void {
  if (Array.isArray(response.errors) && response.errors.length > 0) {
    const formattedMessage = `${response.message || fallbackMessage}:\n${response.errors.map(err => `• ${err}`).join('\n')}`;
    toast.error(formattedMessage, { 
      duration: 5000,
      style: { 
        whiteSpace: 'pre-line',
        maxWidth: '500px'
      }
    });
  } else {
    toast.error(response.message || fallbackMessage);
  }
}
