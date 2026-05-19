namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Role access definition with localized names.
/// </summary>
public class RoleAccess : Entity<int>, IAuditable
{
    public string Code { get; private set; } = null!;
    public string NameRu { get; private set; } = null!;
    public string NameKz { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private readonly List<UserAccess> _userAccesses = new();
    public IReadOnlyCollection<UserAccess> UserAccesses => _userAccesses.AsReadOnly();

    private readonly List<RoleOperationAction> _operationActions = new();
    public IReadOnlyCollection<RoleOperationAction> OperationActions => _operationActions.AsReadOnly();

    private RoleAccess() { }

    public RoleAccess(string code, string nameRu, string nameKz, string nameEn, int createdBy)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Role code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameRu))
            throw new ArgumentException("Russian name is required.", nameof(nameRu));

        Code = code.ToUpperInvariant();
        NameRu = nameRu;
        NameKz = nameKz ?? nameRu;
        NameEn = nameEn ?? nameRu;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void UpdateNames(string nameRu, string nameKz, string nameEn, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(nameRu))
            throw new ArgumentException("Russian name is required.", nameof(nameRu));

        NameRu = nameRu;
        NameKz = nameKz ?? nameRu;
        NameEn = nameEn ?? nameRu;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
