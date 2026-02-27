namespace AWM.Service.Application.Common.Behaviors;

using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using AWM.Service.Domain.Common;

/// <summary>
/// MediatR pipeline behavior that measures the execution time of requests
/// and logs a warning if they exceed a certain threshold (e.g., 500ms).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserProvider _currentUserProvider;

    private const int ThrottleThresholdMilliseconds = 500;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserProvider currentUserProvider)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > ThrottleThresholdMilliseconds)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserProvider.UserId?.ToString() ?? "Anonymous";

            _logger.LogWarning("AWM Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) [User: {UserId}] {@Request}",
                requestName, elapsedMilliseconds, userId, request);
        }

        return response;
    }
}
