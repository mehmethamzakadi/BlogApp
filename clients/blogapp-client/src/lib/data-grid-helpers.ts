/**
 * DataGrid için dinamik query payload oluşturan yardımcı fonksiyonlar
 */

export interface DataGridSort {
  field: string;
  dir: 'asc' | 'desc';
}

export interface DataGridFilters {
  pageIndex: number;
  pageSize: number;
  sort?: DataGridSort;
  search?: string;
}

export interface DynamicFilter {
  Field?: string;
  Operator?: string;
  Value?: string;
  Logic?: string;
  Filters?: DynamicFilter[];
}

export interface DynamicSort {
  Field: string;
  Dir: string;
}

export interface DataGridPayload {
  PaginatedRequest: {
    PageIndex: number;
    PageSize: number;
  };
  DynamicQuery?: {
    Sort?: DynamicSort[];
    Filter?: DynamicFilter;
  };
}

/**
 * Basit tek alanlı arama için DataGrid payload oluşturur
 */
export function buildDataGridPayload(
  filters: DataGridFilters,
  searchField: string = 'Name',
  sortFieldMap?: Record<string, string>
): DataGridPayload {
  const payload: DataGridPayload = {
    PaginatedRequest: {
      PageIndex: filters.pageIndex,
      PageSize: filters.pageSize
    }
  };

  const sort = filters.sort
    ? [
        {
          Field: sortFieldMap?.[filters.sort.field] ?? filters.sort.field,
          Dir: filters.sort.dir
        }
      ]
    : undefined;

  const filter = filters.search
    ? {
        Field: searchField,
        Operator: 'contains',
        Value: filters.search
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

/**
 * Çoklu alanlı arama için DataGrid payload oluşturur
 */
export function buildMultiFieldDataGridPayload(
  filters: DataGridFilters,
  searchFields: string[],
  sortFieldMap?: Record<string, string>
): DataGridPayload {
  const payload: DataGridPayload = {
    PaginatedRequest: {
      PageIndex: filters.pageIndex,
      PageSize: filters.pageSize
    }
  };

  const sort = filters.sort
    ? [
        {
          Field: sortFieldMap?.[filters.sort.field] ?? filters.sort.field,
          Dir: filters.sort.dir
        }
      ]
    : undefined;

  const filter = filters.search
    ? {
        Logic: 'or',
        Filters: searchFields.map((field) => ({
          Field: field,
          Operator: 'contains',
          Value: filters.search!
        }))
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

/**
 * Özel filter ile DataGrid payload oluşturur
 */
export function buildCustomDataGridPayload(
  filters: DataGridFilters,
  customFilter?: DynamicFilter,
  sortFieldMap?: Record<string, string>
): DataGridPayload {
  const payload: DataGridPayload = {
    PaginatedRequest: {
      PageIndex: filters.pageIndex,
      PageSize: filters.pageSize
    }
  };

  const sort = filters.sort
    ? [
        {
          Field: sortFieldMap?.[filters.sort.field] ?? filters.sort.field,
          Dir: filters.sort.dir
        }
      ]
    : undefined;

  if (sort || customFilter) {
    payload.DynamicQuery = {
      ...(sort ? { Sort: sort } : {}),
      ...(customFilter ? { Filter: customFilter } : {})
    };
  }

  return payload;
}
