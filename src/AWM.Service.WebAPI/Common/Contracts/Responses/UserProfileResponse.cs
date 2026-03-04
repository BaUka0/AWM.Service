namespace AWM.Service.WebAPI.Common.Contracts.Responses;

/// <summary>
/// Response model for the GET /api/v1/Users/me endpoint.
/// </summary>
public sealed class UserProfileResponse
{
    public int UserId { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public IReadOnlyList<string> Roles { get; init; } = [];

    /// <summary>
    /// The department the user is scoped to (null for global roles like Admin).
    /// </summary>
    public int? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }

    /// <summary>
    /// The institute the user belongs to.
    /// </summary>
    public int? InstituteId { get; init; }
    public string? InstituteName { get; init; }

    /// <summary>
    /// The currently active academic year.
    /// </summary>
    public int? CurrentAcademicYearId { get; init; }
    public string? CurrentAcademicYearName { get; init; }

    // Staff-specific fields (null for non-staff users)
    public int? StaffId { get; init; }
    public string? Position { get; init; }
    public string? AcademicDegree { get; init; }
    public bool? IsSupervisor { get; init; }

    // Student-specific fields (null for non-student users)
    public int? StudentId { get; init; }
    public string? GroupCode { get; init; }
}
