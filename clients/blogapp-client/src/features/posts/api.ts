import api from '../../lib/axios';
import { normalizePaginatedResponse } from '../../types/api';
import { PostListResponse, PostSummary } from './types';

export async function fetchPublishedPosts(pageIndex = 0, pageSize = 6) {
  const payload = {
    PaginatedRequest: {
      PageIndex: pageIndex,
      PageSize: pageSize
    },
    DynamicQuery: {
      Filter: {
        Field: 'IsPublished',
        Operator: 'eq',
        Value: String(true)
      },
      Sort: [
        {
          Field: 'Id',
          Dir: 'desc'
        }
      ]
    }
  };

  const response = await api.post<PostListResponse>('/Post/GetPaginatedList', payload);
  return normalizePaginatedResponse<PostSummary>(response.data);
}
