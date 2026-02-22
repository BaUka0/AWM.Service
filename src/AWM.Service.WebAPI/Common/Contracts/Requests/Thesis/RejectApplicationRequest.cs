namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using FluentValidation;

/// <summary>
/// Request contract for rejecting a topic application.
/// </summary>
public sealed record RejectApplicationRequest
{
    /// <summary>
    /// Reason for rejecting the application.
    /// </summary>
    /// <example>The student does not meet the prerequisites for this topic.</example>
    public string RejectReason { get; init; } = null!;
}

/// <summary>
/// Validator for RejectApplicationRequest.
/// </summary>
public sealed class RejectApplicationRequestValidator : AbstractValidator<RejectApplicationRequest>
{
    public RejectApplicationRequestValidator()
    {
        RuleFor(x => x.RejectReason)
            .NotEmpty()
            .WithMessage("RejectReason is required.")
            .MaximumLength(2000)
            .WithMessage("RejectReason must not exceed 2000 characters.");
    }
}

