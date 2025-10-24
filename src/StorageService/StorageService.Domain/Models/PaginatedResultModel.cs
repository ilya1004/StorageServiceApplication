namespace StorageService.Domain.Models;

public record PaginatedResultModel<TEntity>
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<TEntity> Items { get; set; } = [];
}