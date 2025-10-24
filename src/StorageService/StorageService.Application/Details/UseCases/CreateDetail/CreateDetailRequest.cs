using AutoMapper;
using MediatR;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Application.Details.UseCases.CreateDetail;

public class CreateDetailRequest : IRequestHandler<CreateDetailCommand, DetailCoreDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDetailRequest(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DetailCoreDto> Handle(CreateDetailCommand request, CancellationToken cancellationToken)
    {
        var detail = await _unitOfWork.DetailsRepository
            .FirstOrDefaultAsync(x => x.NomenclatureCode == request.NomenclatureCode, cancellationToken);

        if (detail is not null)
        {
            throw new AlreadyExistsException("Detail with same nomenclature code already exists");
        }

        var newDetail = new Detail
        {
            NomenclatureCode = request.NomenclatureCode,
            Name = request.Name,
            Count = request.Count,
            StorekeeperId = request.StorekeeperId,
            IsDeleted = false,
            CreatedAtDate = request.CreatedAtDate,
        };

        await _unitOfWork.DetailsRepository.AddAsync(newDetail, cancellationToken);
        await _unitOfWork.SaveAllAsync(cancellationToken);

        return _mapper.Map<DetailCoreDto>(newDetail);
    }
}