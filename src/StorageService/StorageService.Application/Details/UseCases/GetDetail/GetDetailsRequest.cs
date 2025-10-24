using AutoMapper;
using MediatR;
using StorageService.Application.Details.Dtos;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Models;

namespace StorageService.Application.Details.UseCases.GetDetail;

public class GetDetailsRequest : IRequestHandler<GetDetailsQuery, PaginatedResultModel<DetailCoreDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDetailsRequest(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResultModel<DetailCoreDto>> Handle(GetDetailsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var details = await _unitOfWork.DetailsRepository.PaginatedListAsync(
                offset,
                request.PageSize,
                x => !x.IsDeleted,
                cancellationToken,
                x => x.Storekeeper);

        var totalCount = await _unitOfWork.DetailsRepository.CountAsync(x => !x.IsDeleted, cancellationToken);

        return new PaginatedResultModel<DetailCoreDto>
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Items = details.Select(_mapper.Map<DetailCoreDto>).ToList(),
        };
    }
}