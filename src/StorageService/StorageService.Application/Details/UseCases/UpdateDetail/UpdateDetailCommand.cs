using MediatR;
using StorageService.Application.Details.Dtos;

namespace StorageService.Application.Details.UseCases.UpdateDetail;

public sealed record UpdateDetailCommand(
    int Id,
    string NomenclatureCode,
    string Name,
    int Count,
    int StorekeeperId,
    DateTime CreatedAtDate) : IRequest<DetailCoreDto>;