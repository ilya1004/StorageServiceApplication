namespace StorageService.API.RequestDtos.Input;

public record CreateDetailDto
{
    public string NomenclatureCode { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int StorekeeperId { get; set; }
    public DateTime CreatedAtDate { get; set; }
}