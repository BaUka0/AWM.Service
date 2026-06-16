using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetReconciliationSummary;

/// <summary>
/// Query to get the reconciliation summary for a department/semester.
/// Provides aggregate statistics and filterable list of topics for department review.
/// </summary>
public record GetReconciliationSummaryQuery(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null) : IRequest<Result<TopicReconciliationSummaryDto>>;
