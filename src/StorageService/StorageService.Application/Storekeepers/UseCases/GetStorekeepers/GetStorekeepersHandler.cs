using MediatR;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Models;

namespace StorageService.Application.Storekeepers.UseCases.GetStorekeepers;

public class GetStorekeepersHandler
    : IRequestHandler<GetStorekeepersQuery, PaginatedResultModel<StorekeeperWithDetailsCountDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStorekeepersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultModel<StorekeeperWithDetailsCountDto>> Handle(
        GetStorekeepersQuery request,
        CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var result = await _unitOfWork.StorekeepersRepository.ListWithProjectionAsync(
            offset,
            request.PageSize,
            x => new StorekeeperWithDetailsCountDto
            {
                Id = x.Id,
                DetailsCount = x.Details
                    .Where(d => d.StorekeeperId == x.Id)
                    .Sum(d => d.Count),
                FullName = x.FullName,
            },
            x => !x.IsDeleted,
            cancellationToken,
            x => x.Details);

        var count = await _unitOfWork.StorekeepersRepository.CountAsync(null, cancellationToken);

        return new PaginatedResultModel<StorekeeperWithDetailsCountDto>
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = count,
            Items = result.ToList()
        };
    }
}