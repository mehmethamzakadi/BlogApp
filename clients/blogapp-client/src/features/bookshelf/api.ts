import api from '../../lib/axios';
import { ApiResult, normalizeApiResult, normalizePaginatedResponse } from '../../types/api';
import { buildCustomDataGridPayload, DynamicFilter } from '../../lib/data-grid-helpers';
import { BookshelfFormValues, BookshelfItem, BookshelfListResponse, BookshelfTableFilters } from './types';

type BookshelfApiResponse = ApiResult<BookshelfItem>;

const sortFieldMap: Record<string, string> = {
  title: 'Title',
  author: 'Author',
  publisher: 'Publisher',
  pageCount: 'PageCount',
  isRead: 'IsRead',
  readDate: 'ReadDate',
  createdDate: 'CreatedDate'
};

function buildBookshelfFilter(filters: BookshelfTableFilters): DynamicFilter | undefined {
  const filterParts: DynamicFilter[] = [];

  if (filters.search && filters.search.trim().length > 0) {
    const searchValue = filters.search.trim();
    filterParts.push({
      Logic: 'or',
      Filters: [
        {
          Field: 'Title',
          Operator: 'contains',
          Value: searchValue
        },
        {
          Field: 'Author',
          Operator: 'contains',
          Value: searchValue
        },
        {
          Field: 'Publisher',
          Operator: 'contains',
          Value: searchValue
        },
        {
          Field: 'Notes',
          Operator: 'contains',
          Value: searchValue
        }
      ]
    });
  }

  if (filters.status === 'read' || filters.status === 'unread') {
    filterParts.push({
      Field: 'IsRead',
      Operator: 'eq',
      Value: filters.status === 'read' ? 'true' : 'false'
    });
  }

  if (filterParts.length === 0) {
    return undefined;
  }

  if (filterParts.length === 1) {
    return filterParts[0];
  }

  return {
    Logic: 'and',
    Filters: filterParts
  };
}

export async function fetchBookshelfItems(
  filters: BookshelfTableFilters
): Promise<BookshelfListResponse> {
  const response = await api.post('/bookshelf/search', buildCustomDataGridPayload(filters, buildBookshelfFilter(filters), sortFieldMap));
  return normalizePaginatedResponse<BookshelfItem>(response.data);
}

export async function createBookshelfItem(values: BookshelfFormValues) {
  const response = await api.post<ApiResult>('/bookshelf', {
    Title: values.title,
    Author: values.author?.trim() || null,
    Publisher: values.publisher?.trim() || null,
    PageCount: values.pageCount ? Number(values.pageCount) : null,
    IsRead: values.isRead,
    Notes: values.notes?.trim() || null,
    ReadDate: values.isRead && values.readDate ? new Date(values.readDate).toISOString() : null
  });

  return normalizeApiResult(response.data);
}

export async function updateBookshelfItem(id: string, values: BookshelfFormValues) {
  const response = await api.put<ApiResult>(`/bookshelf/${id}`, {
    Id: id,
    Title: values.title,
    Author: values.author?.trim() || null,
    Publisher: values.publisher?.trim() || null,
    PageCount: values.pageCount ? Number(values.pageCount) : null,
    IsRead: values.isRead,
    Notes: values.notes?.trim() || null,
    ReadDate: values.isRead && values.readDate ? new Date(values.readDate).toISOString() : null
  });

  return normalizeApiResult(response.data);
}

export async function deleteBookshelfItem(id: string) {
  const response = await api.delete<ApiResult>(`/bookshelf/${id}`);
  return normalizeApiResult(response.data);
}

export async function getBookshelfItemById(id: string): Promise<BookshelfItem> {
  const response = await api.get<BookshelfApiResponse>(`/bookshelf/${id}`);
  const result = normalizeApiResult<BookshelfItem>(response.data);

  if (!result.success || !result.data) {
    throw {
      ...result,
      success: false
    };
  }

  return result.data;
}
