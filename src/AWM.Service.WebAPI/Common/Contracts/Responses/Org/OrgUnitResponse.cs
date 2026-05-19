namespace AWM.Service.WebAPI.Common.Contracts.Responses.Org;

using System.Collections.Generic;

/// <summary>
/// Response contract for organizational unit (department or institute).
/// </summary>
public sealed record OrgUnitResponse
{
    public int Id { get; init; }
    public int? ParentId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public int TypeId { get; init; }
    public IReadOnlyCollection<OrgUnitResponse>? Children { get; init; }
}
