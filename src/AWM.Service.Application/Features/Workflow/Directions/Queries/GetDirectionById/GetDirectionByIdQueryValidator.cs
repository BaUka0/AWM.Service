using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Directions.Queries.GetDirectionById;

/// <summary>
/// Validator for the <see cref="GetDirectionByIdQuery"/>.
/// </summary>
public sealed class GetDirectionByIdQueryValidator : AbstractValidator<GetDirectionByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDirectionByIdQueryValidator"/> class.
    /// </summary>
    public GetDirectionByIdQueryValidator()
    {
        RuleFor(v => v.Id).GreaterThan(0);
    }
}
