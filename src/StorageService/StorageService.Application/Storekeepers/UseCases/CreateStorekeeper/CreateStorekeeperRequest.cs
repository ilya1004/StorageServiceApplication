using AutoMapper;
using MediatR;
using StorageService.Application.Exceptions;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Application.Storekeepers.UseCases.CreateStorekeeper;

public class CreateStorekeeperRequest : IRequestHandler<CreateStorekeeperCommand, StorekeeperCoreDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStorekeeperRequest(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StorekeeperCoreDto> Handle(CreateStorekeeperCommand request, CancellationToken cancellationToken)
    {
        var storekeeper = await _unitOfWork.StorekeepersRepository
            .FirstOrDefaultAsync(x => x.FullName == request.FullName && !x.IsDeleted, cancellationToken);

        if (storekeeper is not null)
        {
            throw new AlreadyExistsException("Storekeeper with this Full name already exists");
        }

        var newStorekeeper = new Storekeeper
        {
            FullName = request.FullName,
            IsDeleted = false,
        };

        await _unitOfWork.StorekeepersRepository.AddAsync(newStorekeeper, cancellationToken);
        await _unitOfWork.SaveAllAsync(cancellationToken);

        return _mapper.Map<StorekeeperCoreDto>(newStorekeeper);
    }
}