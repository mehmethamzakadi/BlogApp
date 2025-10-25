import api from '../../lib/axios';
import { ApiResult, PaginatedListResponse, DataGridRequest } from '../../types/api';
import { ActivityLog } from './types';

export async function getActivityLogs(request: DataGridRequest): Promise<PaginatedListResponse<ActivityLog>> {
  const response = await api.post<ApiResult<PaginatedListResponse<ActivityLog>>>(
    '/ActivityLogs/search',
    request
  );
  return response.data.data;
}
