namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

public sealed record AutoDistributeStudentsRequest(
    int OrgUnitId,
    int SemesterId,
    int CommissionTypeId,
    int? PreDefenseNumber = null,
    int? SpecialityId = null);
