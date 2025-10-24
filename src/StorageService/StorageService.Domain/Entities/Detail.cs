using StorageService.Domain.Primitives;

namespace StorageService.Domain.Entities;

public class Detail : Entity
{
    public string NomenclatureCode { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int StorekeeperId { get; set; }
    public Storekeeper Storekeeper { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtDate { get; set; }
    public DateTime? DeletedAtDate { get; set; }
}