using AutoMapper;
using MediatR;
using StorageService.Application.Exceptions;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Storekeepers.UseCases.GetStorekeeperById;

public class GetStorekeeperByIdHandler : IRequestHandler<GetStorekeeperByIdQuery, StorekeeperCoreDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStorekeeperByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StorekeeperCoreDto> Handle(GetStorekeeperByIdQuery request, CancellationToken cancellationToken)
    {
        var storekeeper = await _unitOfWork.StorekeepersRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (storekeeper is null)
        {
            throw new NotFoundException("Storekeeper is not found");
        }

        return _mapper.Map<StorekeeperCoreDto>(storekeeper);
    }
}