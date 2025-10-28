using AutoMapper;
using MediatR;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Details.UseCases.UpdateDetail;

public class UpdateDetailHandler : IRequestHandler<UpdateDetailCommand, DetailCoreDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DetailCoreDto> Handle(UpdateDetailCommand request, CancellationToken cancellationToken)
    {
        var detail = await _unitOfWork.DetailsRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (detail is null)
        {
            throw new NotFoundException("Detail not found");
        }

        var existingWithSameCode = await _unitOfWork.DetailsRepository
            .AnyAsync(x =>
                    x.NomenclatureCode == request.NomenclatureCode &&
                    x.Id != request.Id &&
                    !x.IsDeleted,
                cancellationToken);

        if (existingWithSameCode)
        {
            throw new AlreadyExistsException("Detail with same nomenclature code already exists");
        }

        detail.NomenclatureCode = request.NomenclatureCode;
        detail.Name = request.Name;
        detail.Count = request.Count;
        detail.StorekeeperId = request.StorekeeperId;
        detail.CreatedAtDate = request.CreatedAtDate;

        await _unitOfWork.SaveAllAsync(cancellationToken);

        return _mapper.Map<DetailCoreDto>(detail);
    }
}