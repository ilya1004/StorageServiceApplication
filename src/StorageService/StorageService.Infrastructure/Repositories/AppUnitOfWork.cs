using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.Infrastructure.Repositories;

public class AppUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Lazy<IDetailsRepository> _detailsRepository;
    private readonly Lazy<IStorekeeperRepository> _storekeepersRepository;

    public AppUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _detailsRepository = new Lazy<IDetailsRepository>(() =>
            new DetailsRepository(context));
        _storekeepersRepository = new Lazy<IStorekeeperRepository>(() =>
            new StorekeeperRepository(context));
    }

    public IDetailsRepository DetailsRepository => _detailsRepository.Value;
    public IStorekeeperRepository StorekeepersRepository => _storekeepersRepository.Value;
    public async Task SaveAllAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}