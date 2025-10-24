using StorageService.Application.Storekeepers.Dtos;

namespace StorageService.Application.Details.Dtos;

public record DetailCoreDto
{
    public string NomenclatureCode { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int StorekeeperId { get; set; }
    public StorekeeperCoreDto? Storekeeper { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}