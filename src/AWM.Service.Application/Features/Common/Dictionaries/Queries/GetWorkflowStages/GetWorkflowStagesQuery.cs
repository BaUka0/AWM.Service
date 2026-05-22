namespace AWM.Service.Application.Features.Common.Dictionaries.Queries.GetWorkflowStages;

using AWM.Service.Domain.CommonDomain.Entities;
using MediatR;

/// <summary>
/// Query to get all workflow stages (reference dictionary).
/// </summary>
public sealed record GetWorkflowStagesQuery : IRequest<IReadOnlyList<WorkflowStage>>;
