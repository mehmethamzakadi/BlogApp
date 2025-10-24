import toast from 'react-hot-toast';
import { ApiError } from '../types/api';

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

export function handleApiError(error: unknown, fallbackMessage = 'Beklenmeyen bir hata oluştu'): string {
  const message = getApiErrorMessage(error, fallbackMessage);
  toast.error(message);
  return message;
}
