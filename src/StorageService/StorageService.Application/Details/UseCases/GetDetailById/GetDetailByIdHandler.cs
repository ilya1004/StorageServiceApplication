using AutoMapper;
using MediatR;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;

namespace StorageService.Application.Details.UseCases.GetDetailById;

public class GetDetailByIdHandler : IRequestHandler<GetDetailByIdQuery, DetailCoreDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDetailByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DetailCoreDto> Handle(GetDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var detail = await _unitOfWork.DetailsRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (detail is null)
        {
            throw new NotFoundException("Detail is not found");
        }

        return _mapper.Map<DetailCoreDto>(detail);
    }
}