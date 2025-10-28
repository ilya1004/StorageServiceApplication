using MediatR;
using StorageService.Application.Storekeepers.Dtos;

namespace StorageService.Application.Storekeepers.UseCases.GetStorekeeperById;

public sealed record GetStorekeeperByIdQuery(int Id) : IRequest<StorekeeperCoreDto>;