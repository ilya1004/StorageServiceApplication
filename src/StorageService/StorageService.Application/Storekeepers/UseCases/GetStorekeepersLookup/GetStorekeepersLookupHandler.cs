using MediatR;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Storekeepers.UseCases.GetStorekeepersLookup;

public class GetStorekeepersLookupHandler : IRequestHandler<GetStorekeepersLookupQuery, List<StorekeeperCoreDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStorekeepersLookupHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StorekeeperCoreDto>> Handle(GetStorekeepersLookupQuery request, CancellationToken cancellationToken)
    {
        var storekeepers = await _unitOfWork.StorekeepersRepository
            .ListAsync(x => !x.IsDeleted, cancellationToken);

        return storekeepers.Select(x => new StorekeeperCoreDto
        {
            Id = x.Id,
            FullName = x.FullName
        }).ToList();
    }
}