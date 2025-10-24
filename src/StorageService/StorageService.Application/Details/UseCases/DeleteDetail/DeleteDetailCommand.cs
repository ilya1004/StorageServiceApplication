using MediatR;

namespace StorageService.Application.Details.UseCases.DeleteDetail;

public sealed record DeleteDetailCommand(int Id) : IRequest;