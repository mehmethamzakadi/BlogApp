import api from '../../lib/axios';
import { ApiResult, normalizeApiResult } from '../../types/api';
import { LoginRequest, LoginResponse } from './types';

export async function login(request: LoginRequest): Promise<ApiResult<LoginResponse>> {
  const response = await api.post<ApiResult<LoginResponse>>('/auth/login', request);
  return normalizeApiResult<LoginResponse>(response.data);
}

export async function refreshToken(token: string): Promise<ApiResult<LoginResponse>> {
  const response = await api.post<ApiResult<LoginResponse>>('/auth/refresh-token', {
    refreshToken: token
  });
  return normalizeApiResult<LoginResponse>(response.data);
}
