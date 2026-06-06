using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CompleteTopicReconciliation;

/// <summary>
/// Command to complete the topic reconciliation stage for a department/semester.
/// This is an irreversible operation that:
/// 1. Validates all topics are in final reconciliation states (Reconciled, Inactive, Rejected, NeedsRevision excluded)
/// 2. Creates StudentWork entities for each Reconciled topic with accepted students
/// 3. Blocks further modifications to topic assignments
/// </summary>
public record CompleteTopicReconciliationCommand(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null) : IRequest<Result>;
