using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AWM.Service.Domain.Common;
using FluentValidation;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Common.Behaviors;

/// <summary>
/// Caches compiled delegates for creating failure results to avoid reflection overhead.
/// </summary>
internal static class ValidationResponseCache<TResponse>
{
    public static readonly Func<Error, TResponse>? CreateFailure;

    static ValidationResponseCache()
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            var failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Failure" && 
                                     !m.IsGenericMethod && 
                                     m.GetParameters().Length == 1 && 
                                     m.GetParameters()[0].ParameterType == typeof(Error));

            if (failureMethod != null)
            {
                CreateFailure = (Func<Error, TResponse>)Delegate.CreateDelegate(typeof(Func<Error, TResponse>), failureMethod);
            }
        }
        else if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Failure" && 
                                     m.IsGenericMethod && 
                                     m.GetParameters().Length == 1 && 
                                     m.GetParameters()[0].ParameterType == typeof(Error));

            if (failureMethod != null)
            {
                var genericFailureMethod = failureMethod.MakeGenericMethod(innerType);
                CreateFailure = (Func<Error, TResponse>)Delegate.CreateDelegate(typeof(Func<Error, TResponse>), genericFailureMethod);
            }
        }
    }
}

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
            if (ValidationResponseCache<TResponse>.CreateFailure is not null)
            {
                var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
                var error = new Error(ErrorCodes.Validation, errorMessage);
                return ValidationResponseCache<TResponse>.CreateFailure(error);
            }

            // Fallback: throw ValidationException for non-Result types
            throw new ValidationException(failures);
        }

        return await next();
    }
}
