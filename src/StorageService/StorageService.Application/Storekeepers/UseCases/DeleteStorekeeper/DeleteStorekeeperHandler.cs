using MediatR;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Storekeepers.UseCases.DeleteStorekeeper;

public class DeleteStorekeeperHandler : IRequestHandler<DeleteStorekeeperCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStorekeeperHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteStorekeeperCommand request, CancellationToken cancellationToken)
    {
        var storekeeper = await _unitOfWork.StorekeepersRepository.GetByIdAsync(request.Id, cancellationToken);

        if (storekeeper is null)
        {
            throw new NotFoundException("Storekeeper is not found");
        }

        var isRelatedDetailsExists = await _unitOfWork.DetailsRepository
            .AnyAsync(x => x.StorekeeperId == storekeeper.Id, cancellationToken);

        if (isRelatedDetailsExists)
        {
            throw new BadRequestException("You cannot delete storekeeper as he has details");
        }

        storekeeper.IsDeleted = true;

        await _unitOfWork.SaveAllAsync(cancellationToken);
    }
}