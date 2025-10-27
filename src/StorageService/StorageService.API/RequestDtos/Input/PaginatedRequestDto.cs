namespace StorageService.API.RequestDtos.Input;

public sealed record PaginatedRequestDto
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
}