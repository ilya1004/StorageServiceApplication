namespace StorageService.Application.Storekeepers.Dtos;

public record StorekeeperWithDetailsCountDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int DetailsCount { get; set; }
}