namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for direction (list view).
/// </summary>
public record DirectionResponse
{
    public long Id { get; init; }
    public int DepartmentId { get; init; }
    public int SupervisorId { get; init; }
    public int AcademicYearId { get; init; }
    public int WorkTypeId { get; init; }

    public string TitleRu { get; init; } = string.Empty;
    public string? TitleKz { get; init; }
    public string? TitleEn { get; init; }
    public string? DescriptionRu { get; init; }
    public string? DescriptionKz { get; init; }
    public string? DescriptionEn { get; init; }

    public AWM.Service.WebAPI.Common.Contracts.Responses.Common.LocalizedTextResponse Title => new() 
    { 
        Ru = TitleRu, 
        Kk = TitleKz, 
        En = TitleEn 
    };

    public AWM.Service.WebAPI.Common.Contracts.Responses.Common.LocalizedTextResponse Description => new()
    {
        Ru = DescriptionRu ?? string.Empty,
        Kk = DescriptionKz,
        En = DescriptionEn
    };

    public int CurrentStateId { get; init; }
    public string? CurrentStateName { get; init; }
    public string? CurrentStateDisplayName { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public int? ReviewedBy { get; init; }
    public string? ReviewComment { get; init; }

    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
}
