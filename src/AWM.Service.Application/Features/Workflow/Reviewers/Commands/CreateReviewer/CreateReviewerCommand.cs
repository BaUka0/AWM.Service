using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.CreateReviewer;

public record CreateReviewerCommand(
    string FullName,
    string? Position,
    string? AcademicDegree,
    string? Organization,
    string? Email,
    string? Phone) : IRequest<Result<int>>;
