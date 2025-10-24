using MediatR;
using StorageService.Application.Storekeepers.Dtos;

namespace StorageService.Application.Storekeepers.UseCases.CreateStorekeeper;

public sealed record CreateStorekeeperCommand(string FullName) : IRequest<StorekeeperCoreDto>;