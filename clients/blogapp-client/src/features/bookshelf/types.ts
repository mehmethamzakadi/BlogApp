import { PaginatedListResponse } from '../../types/api';

export type BookshelfStatusFilter = 'all' | 'read' | 'unread';

export interface BookshelfItem {
  id: string;
  title: string;
  author?: string | null;
  publisher?: string | null;
  pageCount?: number | null;
  isRead: boolean;
  notes?: string | null;
  readDate?: Date | null;
  createdDate: Date;
}

export type BookshelfListResponse = PaginatedListResponse<BookshelfItem>;

export interface BookshelfFormValues {
  title: string;
  author: string;
  publisher: string;
  pageCount: string;
  isRead: boolean;
  notes: string;
  readDate: string;
}

export interface BookshelfTableFilters {
  search?: string;
  status: BookshelfStatusFilter;
  pageIndex: number;
  pageSize: number;
  sort?: {
    field: string;
    dir: 'asc' | 'desc';
  };
}
