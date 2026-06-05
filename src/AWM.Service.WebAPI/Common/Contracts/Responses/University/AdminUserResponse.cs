namespace AWM.Service.WebAPI.Common.Contracts.Responses.University;

public record AdminUserResponse(
    int Id,
    string Iin,
    string FullName,
    string Email,
    string PositionTitle,
    int? OrgUnitId,
    bool IsActive = true,
    DateTime? CreatedAt = null,
    IReadOnlyList<string> Roles = null
);
