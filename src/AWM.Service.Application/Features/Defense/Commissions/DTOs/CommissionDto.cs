namespace AWM.Service.Application.Features.Defense.Commissions.DTOs;

public record CommissionDto(
    int Id,
    string Name,
    int CommissionTypeId,
    int? PreDefenseNumber,
    int OrgUnitId,
    int? SpecialityId,
    int SemesterId,
    IReadOnlyList<CommissionMemberDto> Members
);

public record CommissionMemberDto(
    int UserId,
    string FullName,
    int RoleType
);
