using MediatR;
using StorageService.Application.Details.Dtos;

namespace StorageService.Application.Details.UseCases.GetDetailById;

public sealed record GetDetailByIdQuery(int Id) : IRequest<DetailCoreDto>;