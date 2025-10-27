export interface PaginatedResult<TEntity> {
  pageNo: number;
  pageSize: number;
  totalCount: number;
  items: TEntity[];
}
