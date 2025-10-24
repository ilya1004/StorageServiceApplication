using MediatR;
using StorageService.Application.Storekeepers.Dtos;

namespace StorageService.Application.Storekeepers.UseCases.GetStorekeepersLookup;

public sealed record GetStorekeepersLookupQuery() : IRequest<List<StorekeeperCoreDto>>;