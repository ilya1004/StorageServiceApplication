using MediatR;
using StorageService.Application.Details.Dtos;
using StorageService.Domain.Models;

namespace StorageService.Application.Details.UseCases.GetDetail;

public sealed record GetDetailsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<DetailCoreDto>>;