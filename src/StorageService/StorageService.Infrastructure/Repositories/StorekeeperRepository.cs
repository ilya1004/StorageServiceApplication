using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.Infrastructure.Repositories;

public class StorekeeperRepository : BaseRepository<Storekeeper>, IStorekeeperRepository
{
    public StorekeeperRepository(ApplicationDbContext context) : base(context)
    {
    }
}