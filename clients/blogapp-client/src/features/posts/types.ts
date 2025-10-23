import { PaginatedListResponse } from '../../types/api';

export interface PostSummary {
  id: number;
  title: string;
  summary: string;
  thumbnail: string;
  isPublished: boolean;
  categoryName: string;
  categoryId: number;
}

export type PostListResponse = PaginatedListResponse<PostSummary>;
