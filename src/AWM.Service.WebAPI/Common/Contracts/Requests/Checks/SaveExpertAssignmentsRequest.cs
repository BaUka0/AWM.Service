using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using System.Collections.Generic;

namespace AWM.Service.WebAPI.Common.Contracts.Requests.Checks;

public record SaveExpertAssignmentsRequest(
    int OrgUnitId,
    List<ExpertAssignmentInput> Assignments);
