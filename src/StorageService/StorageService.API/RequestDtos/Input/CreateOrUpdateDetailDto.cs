namespace StorageService.API.RequestDtos.Input;

public record CreateOrUpdateDetailDto
{
    public string NomenclatureCode { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int StorekeeperId { get; set; }
    public DateTime CreatedAtDate { get; set; }
}