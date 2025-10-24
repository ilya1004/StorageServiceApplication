using MediatR;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Domain.Models;

namespace StorageService.Application.Storekeepers.UseCases.GetStorekeepers;

public sealed record GetStorekeepersQuery(int PageNo, int PageSize)
    : IRequest<PaginatedResultModel<StorekeeperWithDetailsCountDto>>;