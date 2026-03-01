namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

using System.Collections.Generic;

public class ApproveSupervisorsRequest
{
    public int DepartmentId { get; set; }
    public IReadOnlyList<int> StaffIds { get; set; } = new List<int>();
}
