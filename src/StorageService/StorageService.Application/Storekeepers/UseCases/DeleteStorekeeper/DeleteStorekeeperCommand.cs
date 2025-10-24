using MediatR;

namespace StorageService.Application.Storekeepers.UseCases.DeleteStorekeeper;

public sealed record DeleteStorekeeperCommand(int Id) : IRequest;