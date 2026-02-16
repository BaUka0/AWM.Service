using FluentValidation;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            // Check if TResponse is Result<T> to return validation error properly
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
                var error = new Error("400", errorMessage);

                // Create Result.Failure<T> using reflection
                var innerType = responseType.GetGenericArguments()[0];
                var failureMethod = typeof(Result)
                    .GetMethods()
                    .First(m => m.Name == "Failure" && m.IsGenericMethod && m.GetParameters().Length == 1)
                    .MakeGenericMethod(innerType);

                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
            }

            // Fallback: throw ValidationException for non-Result types
            throw new ValidationException(failures);
        }

        return await next();
    }
}
