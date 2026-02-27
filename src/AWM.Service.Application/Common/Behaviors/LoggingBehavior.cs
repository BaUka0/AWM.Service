namespace AWM.Service.Application.Common.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using AWM.Service.Domain.Common;

/// <summary>
/// MediatR pipeline behavior that logs all incoming requests and their payloads.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserProvider _currentUserProvider;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserProvider currentUserProvider)
    {
        _logger = logger;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserProvider.UserId?.ToString() ?? "Anonymous";

        _logger.LogInformation("AWM Request Handling: {Name} [User: {UserId}] {@Request}",
            requestName, userId, request);

        var response = await next();

        _logger.LogInformation("AWM Request Handled: {Name} [User: {UserId}]",
            requestName, userId);

        return response;
    }
}
