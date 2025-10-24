export interface ApiResult<T = undefined> {
  success: boolean;
  message: string;
  data: T;
}

export interface PaginatedListResponse<T> {
  items: T[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface PaginatedRequest {
  pageIndex: number;
  pageSize: number;
}

export interface SortDescriptor {
  field: string;
  dir: 'asc' | 'desc';
}

export interface FilterDescriptor {
  field: string;
  operator: string;
  value?: string | number | boolean;
  logic?: string;
  filters?: FilterDescriptor[];
}

export interface DataGridRequest {
  paginatedRequest: PaginatedRequest;
  dynamicQuery?: {
    sort?: SortDescriptor[];
    filter?: FilterDescriptor;
  };
}

export function normalizeApiResult<T>(data: any): ApiResult<T> {
  if (typeof data === 'string') {
    return {
      success: true,
      message: data,
      data: undefined as T
    };
  }

  if (data && typeof data === 'object' && 'success' in data) {
    return data as ApiResult<T>;
  }

  return {
    success: Boolean(data?.Success),
    message: data?.Message ?? '',
    data: data?.Data as T
  };
}

export function normalizePaginatedResponse<T>(data: any): PaginatedListResponse<T> {
  if (data && typeof data === 'object' && 'items' in data) {
    return data as PaginatedListResponse<T>;
  }

  return {
    items: (data?.Items ?? []) as T[],
    size: Number(data?.Size ?? data?.PageSize ?? 0),
    index: Number(data?.Index ?? data?.PageIndex ?? 0),
    count: Number(data?.Count ?? 0),
    pages: Number(data?.Pages ?? 0),
    hasPrevious: Boolean(data?.HasPrevious ?? false),
    hasNext: Boolean(data?.HasNext ?? false)
  };
}
