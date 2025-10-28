using MediatR;
using Microsoft.Extensions.Logging;

namespace StorageService.Application.PipelineBehaviors;

public class LoggingBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling request {RequestName} with data: {@Request} [{@DateTimeUtc}]",
            requestName, request, DateTime.UtcNow);

        try
        {
            var response = await next(cancellationToken);

            _logger.LogInformation("Request {RequestName} completed [{@DateTimeUtc}]", requestName, DateTime.UtcNow);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling request {RequestName} [{@DateTimeUtc}]",
                requestName, DateTime.UtcNow);
            throw;
        }
    }
}