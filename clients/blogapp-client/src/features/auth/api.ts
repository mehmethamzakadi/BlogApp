import api from '../../lib/axios';
import { ApiResult, normalizeApiResult } from '../../types/api';
import { LoginRequest, LoginResponse, RegisterRequest } from './types';

export async function login(request: LoginRequest): Promise<ApiResult<LoginResponse>> {
  const response = await api.post<ApiResult<LoginResponse>>('/auth/login', request);
  return normalizeApiResult<LoginResponse>(response.data);
}

export async function register(request: RegisterRequest): Promise<ApiResult<null>> {
  const response = await api.post<ApiResult<null>>('/auth/register', request);
  return normalizeApiResult<null>(response.data);
}

export async function refreshSession(): Promise<ApiResult<LoginResponse>> {
  const response = await api.post<ApiResult<LoginResponse>>('/auth/refresh-token');
  return normalizeApiResult<LoginResponse>(response.data);
}

export async function logout(): Promise<ApiResult<unknown>> {
  const response = await api.post<ApiResult<unknown>>('/auth/logout');
  return normalizeApiResult<unknown>(response.data);
}
