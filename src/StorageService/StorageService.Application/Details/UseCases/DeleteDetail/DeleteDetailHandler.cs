using MediatR;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Details.UseCases.DeleteDetail;

public class DeleteDetailHandler : IRequestHandler<DeleteDetailCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDetailHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDetailCommand request, CancellationToken cancellationToken)
    {
        var detail = await _unitOfWork.DetailsRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (detail is null)
        {
            throw new NotFoundException("Detail is not found");
        }

        detail.IsDeleted = true;
        detail.DeletedAtDate = DateTime.UtcNow;

        await _unitOfWork.SaveAllAsync(cancellationToken);
    }
}