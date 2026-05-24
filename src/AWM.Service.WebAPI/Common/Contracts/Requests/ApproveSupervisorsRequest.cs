namespace AWM.Service.WebAPI.Common.Contracts.Requests;

using System.Collections.Generic;

public record ApproveSupervisorsRequest(
    int SemesterId,
    int? SpecialityId,
    List<SupervisorAssignmentRequest> Assignments
);
