namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

using System.Collections.Generic;

public record ApproveSupervisorsRequest
{
    public int DepartmentId { get; init; }
    public IReadOnlyList<int> StaffIds { get; init; } = new List<int>();
}
