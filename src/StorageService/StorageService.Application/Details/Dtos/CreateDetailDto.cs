namespace StorageService.Application.Details.Dtos;

public record CreateDetailDto
{
    public string NomenclatureCode { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int StorekeeperId { get; set; }
}