import { PaginatedListResponse } from '../../types/api';

export interface PostSummary {
  id: number;
  title: string;
  summary: string;
  body?: string; // Opsiyonel body alanı eklendi
  thumbnail: string;
  isPublished: boolean;
  categoryName: string;
  categoryId: number;
  createdDate: Date;
}

export type PostListResponse = PaginatedListResponse<PostSummary>;

export interface Post {
  id: number;
  title: string;
  body: string;
  summary: string;
  thumbnail: string;
  isPublished: boolean;
  categoryName: string;
  categoryId: number;
  createdDate: Date;
}

export type PostManagementListResponse = PaginatedListResponse<Post>;

export interface PostFormValues {
  title: string;
  body: string;
  summary: string;
  thumbnail: string;
  isPublished: boolean;
  categoryId: number;
}

export interface PostTableFilters {
  search?: string;
  pageIndex: number;
  pageSize: number;
  sort?: {
    field: string;
    dir: 'asc' | 'desc';
  };
}
