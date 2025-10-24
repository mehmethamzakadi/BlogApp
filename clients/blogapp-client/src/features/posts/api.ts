import api from '../../lib/axios';
import { ApiResult, normalizeApiResult, normalizePaginatedResponse } from '../../types/api';
import {
  Post,
  PostFormValues,
  PostListResponse,
  PostManagementListResponse,
  PostSummary,
  PostTableFilters
} from './types';

function buildPostDataGridPayload(filters: PostTableFilters) {
  const payload: Record<string, unknown> = {
    PaginatedRequest: {
      PageIndex: filters.pageIndex,
      PageSize: filters.pageSize
    }
  };

  const sortFieldMap: Record<string, string> = {
    id: 'Id',
    title: 'Title',
    categoryName: 'Category.Name',
    isPublished: 'IsPublished'
  };

  const sort = filters.sort
    ? [
        {
          Field: sortFieldMap[filters.sort.field] ?? filters.sort.field,
          Dir: filters.sort.dir
        }
      ]
    : undefined;

  const filter = filters.search
    ? {
        Field: 'Title',
        Logic: 'or',
        Filters: [
          {
            Field: 'Title',
            Operator: 'contains',
            Value: filters.search
          },
          {
            Field: 'Summary',
            Operator: 'contains',
            Value: filters.search
          }
        ]
      }
    : undefined;

  if (sort || filter) {
    payload.DynamicQuery = {
      ...(sort ? { Sort: sort } : {}),
      ...(filter ? { Filter: filter } : {})
    };
  }

  return payload;
}

export async function fetchPublishedPosts(pageIndex = 0, pageSize = 6) {
  const response = await api.get<PostListResponse>('/Post/GetList', {
    params: {
      PageIndex: pageIndex,
      PageSize: pageSize
    }
  });

  return normalizePaginatedResponse<PostSummary>(response.data);
}

export async function fetchPosts(filters: PostTableFilters): Promise<PostManagementListResponse> {
  const response = await api.post('/Post/GetPaginatedList', buildPostDataGridPayload(filters));
  return normalizePaginatedResponse<Post>(response.data);
}

export async function createPost(values: PostFormValues) {
  const response = await api.post<ApiResult>('/Post/Create', {
    Title: values.title,
    Body: values.body,
    Summary: values.summary,
    Thumbnail: values.thumbnail,
    IsPublished: values.isPublished,
    CategoryId: values.categoryId
  });

  return normalizeApiResult(response.data);
}

export async function updatePost(id: number, values: PostFormValues) {
  const response = await api.put<ApiResult>('/Post/Update', {
    Id: id,
    Title: values.title,
    Body: values.body,
    Summary: values.summary,
    Thumbnail: values.thumbnail,
    IsPublished: values.isPublished,
    CategoryId: values.categoryId
  });

  return normalizeApiResult(response.data);
}

export async function deletePost(id: number) {
  const response = await api.delete<ApiResult>(`/Post/Delete/${id}`);
  return normalizeApiResult(response.data);
}
