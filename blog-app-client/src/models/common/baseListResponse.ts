export type BaseListResponse<T> = {
  items: T[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasPrevious: boolean;
  hasNext: boolean;
};
