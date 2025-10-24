import api from '../../lib/axios';
import { normalizePaginatedResponse } from '../../types/api';
import { PostListResponse, PostSummary } from './types';

export async function fetchPublishedPosts(pageIndex = 0, pageSize = 6) {
  const response = await api.get<PostListResponse>('/Post/GetList', {
    params: {
      PageIndex: pageIndex,
      PageSize: pageSize
    }
  });
  return normalizePaginatedResponse<PostSummary>(response.data);
}
