using MediatR;
using StorageService.Application.Details.Dtos;

namespace StorageService.Application.Details.UseCases.CreateDetail;

public sealed record CreateDetailCommand(
    string NomenclatureCode,
    string Name,
    int Count,
    int StorekeeperId,
    DateTime CreatedAtDate) : IRequest<DetailCoreDto>;