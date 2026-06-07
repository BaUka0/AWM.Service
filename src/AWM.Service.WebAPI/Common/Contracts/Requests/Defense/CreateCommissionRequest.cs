namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

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
    string? Name,
    int? CommissionTypeId,
    int? PreDefenseNumber,
    int? SpecialityId,
    int? ChairmanUserId,
    int? SecretaryUserId,
    List<int>? MemberUserIds
);
