namespace AWM.Service.WebAPI.Common.Contracts.Requests;

using System.Collections.Generic;

public record ApproveEmployeesRequest(
    int SemesterId,
    int? SpecialityId,
    List<EmployeeAssignmentRequest> Assignments
);
