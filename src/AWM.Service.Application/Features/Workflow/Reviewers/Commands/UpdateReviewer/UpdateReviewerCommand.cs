using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.UpdateReviewer;

public record UpdateReviewerCommand(
    int Id,
    string FullName,
    string? Position,
    string? AcademicDegree,
    string? Organization,
    string? Email,
    string? Phone) : IRequest<Result>;
