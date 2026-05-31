namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

public record ApprovePreDefensePeriodsRequest(
    int OrgUnitId,
    int SemesterId
);

public record CreateCommissionRequest(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId,
    int CommissionTypeId,
    int? PreDefenseNumber,
    string? Name,
    int ChairmanUserId,
    int SecretaryUserId,
    List<int>? MemberUserIds = null
);

public record UpdateCommissionRequest(
    string? Name
);
