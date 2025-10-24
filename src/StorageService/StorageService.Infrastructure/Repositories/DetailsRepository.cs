using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.Infrastructure.Repositories;

public class DetailsRepository : BaseRepository<Detail>, IDetailsRepository
{
    public DetailsRepository(ApplicationDbContext context) : base(context)
    {
    }
}