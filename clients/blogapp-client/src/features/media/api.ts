import api from '../../lib/axios';
import { ApiResult, normalizeApiResult } from '../../types/api';
import { ImageUploadOptions, UploadedImage } from './types';

export async function uploadImage(file: File, options?: ImageUploadOptions) {
  const formData = new FormData();
  formData.append('file', file);

  if (options?.scope) {
    formData.append('scope', options.scope);
  }

  if (options?.title) {
    formData.append('title', options.title);
  }

  if (options?.maxWidth) {
    formData.append('maxWidth', options.maxWidth.toString());
  }

  if (options?.maxHeight) {
    formData.append('maxHeight', options.maxHeight.toString());
  }

  if (options?.resizeMode) {
    const mode = options.resizeMode === 'crop' ? 'Crop' : 'Fit';
    formData.append('resizeMode', mode);
  }

  const response = await api.post<ApiResult<UploadedImage>>('/images', formData);
  return normalizeApiResult<UploadedImage>(response.data);
}
