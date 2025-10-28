using MediatR;
using StorageService.Application.Storekeepers.Dtos;

namespace StorageService.Application.Storekeepers.UseCases.UpdateStorekeeper;

public sealed record UpdateStorekeeperCommand(int Id, string FullName) : IRequest<StorekeeperCoreDto>;