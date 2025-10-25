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
        Operator: 'contains',
        Value: filters.search,
        Logic: 'or',
        Filters: [
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

export interface FetchPublishedPostsParams {
  pageIndex?: number;
  pageSize?: number;
  categoryId?: number;
}

export async function fetchPublishedPosts({
  pageIndex = 0,
  pageSize = 6,
  categoryId
}: FetchPublishedPostsParams = {}) {
  const endpoint = '/post';
  const params: Record<string, number> = {
    PageIndex: pageIndex,
    PageSize: pageSize
  };

  if (categoryId != null) {
    params.CategoryId = categoryId;
  }

  const response = await api.get<PostListResponse>(endpoint, {
    params
  });

  return normalizePaginatedResponse<PostSummary>(response.data);
}

export async function fetchPosts(filters: PostTableFilters): Promise<PostManagementListResponse> {
  const response = await api.post('/post/search', buildPostDataGridPayload(filters));
  return normalizePaginatedResponse<Post>(response.data);
}

export async function createPost(values: PostFormValues) {
  const response = await api.post<ApiResult>('/post', {
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
  const response = await api.put<ApiResult>(`/post/${id}`, {
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
  const response = await api.delete<ApiResult>(`/post/${id}`);
  return normalizeApiResult(response.data);
}

export async function getPostById(id: number): Promise<Post> {
  const response = await api.get<ApiResult<Post>>(`/post/${id}`);
  const result = normalizeApiResult<Post>(response.data);

  if (!result.success || !result.data) {
    throw {
      ...result,
      success: false
    };
  }

  return result.data;
}

export interface DashboardStatistics {
  totalPosts: number;
  publishedPosts: number;
  draftPosts: number;
  totalCategories: number;
  postsLast7Days: number;
  postsLast30Days: number;
}

export async function fetchStatistics(): Promise<DashboardStatistics> {
  const response = await api.get<DashboardStatistics>('/dashboard/statistics');
  return response.data;
}

export interface ActivityDto {
  id: number;
  activityType: string;
  entityType: string;
  entityId?: number;
  title: string;
  timestamp: string;
  userName?: string;
}

export interface RecentActivitiesResponse {
  activities: ActivityDto[];
}

export async function fetchRecentActivities(count: number = 10): Promise<ActivityDto[]> {
  const response = await api.get<RecentActivitiesResponse>(`/dashboard/activities?count=${count}`);
  return response.data.activities;
}
