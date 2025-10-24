using StorageService.Domain.Entities;

namespace StorageService.Domain.Abstractions.Data;

public interface IUnitOfWork
{
    public IDetailsRepository DetailsRepository { get; }
    public IStorekeeperRepository StorekeepersRepository { get; }

    public Task SaveAllAsync(CancellationToken cancellationToken = default);
}