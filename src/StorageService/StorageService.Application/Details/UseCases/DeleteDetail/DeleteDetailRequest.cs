using MediatR;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Details.UseCases.DeleteDetail;

public class DeleteDetailRequest : IRequestHandler<DeleteDetailCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDetailRequest(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDetailCommand request, CancellationToken cancellationToken)
    {
        var detail = await _unitOfWork.DetailsRepository.GetByIdAsync(request.Id, cancellationToken);

        if (detail is null)
        {
            throw new NotFoundException("Detail is not found");
        }

        detail.IsDeleted = true;

        await _unitOfWork.SaveAllAsync(cancellationToken);
    }
}