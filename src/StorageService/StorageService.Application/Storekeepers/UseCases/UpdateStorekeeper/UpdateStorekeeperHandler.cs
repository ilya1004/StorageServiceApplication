using AutoMapper;
using MediatR;
using StorageService.Application.Exceptions;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Storekeepers.UseCases.UpdateStorekeeper;

public class UpdateStorekeeperHandler : IRequestHandler<UpdateStorekeeperCommand, StorekeeperCoreDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateStorekeeperHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StorekeeperCoreDto> Handle(UpdateStorekeeperCommand request, CancellationToken cancellationToken)
    {
        var storekeeper = await _unitOfWork.StorekeepersRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (storekeeper is null)
        {
            throw new NotFoundException("Storekeeper is not found");
        }

        var existingWithSameName = await _unitOfWork.StorekeepersRepository
            .AnyAsync(x =>
                    x.FullName == request.FullName &&
                    x.Id != request.Id &&
                    !x.IsDeleted,
                cancellationToken);

        if (existingWithSameName)
        {
            throw new AlreadyExistsException("Storekeeper with this Full name already exists");
        }

        storekeeper.FullName = request.FullName;

        await _unitOfWork.SaveAllAsync(cancellationToken);

        return _mapper.Map<StorekeeperCoreDto>(storekeeper);
    }
}