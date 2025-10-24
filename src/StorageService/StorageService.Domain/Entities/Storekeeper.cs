using StorageService.Domain.Primitives;

namespace StorageService.Domain.Entities;

public class Storekeeper : Entity
{
    public string FullName { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Detail> Details { get; set; }
}