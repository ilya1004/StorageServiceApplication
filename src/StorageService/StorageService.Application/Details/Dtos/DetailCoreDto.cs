using StorageService.Application.Storekeepers.Dtos;

namespace StorageService.Application.Details.Dtos;

public record DetailCoreDto
{
    public int Id { get; set; }
    public string NomenclatureCode { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int StorekeeperId { get; set; }
    public StorekeeperCoreDto? Storekeeper { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtDate { get; set; }
    public DateTime? DeletedAtDate { get; set; }
}