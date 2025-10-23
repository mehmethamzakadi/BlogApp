import api from '../../lib/axios';
import { ApiResult, normalizeApiResult } from '../../types/api';
import { LoginRequest, LoginResponse } from './types';

export async function login(request: LoginRequest): Promise<ApiResult<LoginResponse>> {
  const response = await api.post<ApiResult<LoginResponse>>('/Auth/Login', request);
  return normalizeApiResult<LoginResponse>(response.data);
}
