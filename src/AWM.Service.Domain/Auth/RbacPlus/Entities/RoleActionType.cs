namespace AWM.Service.Domain.Auth.RbacPlus.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Atomic action types (Read, Create, Update, Delete).
/// </summary>
public class RoleActionType : Entity<int>
{
    public string Code { get; private set; } = null!;
    public string NameRu { get; private set; } = null!;
    public string NameKz { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;

    private readonly List<RoleOperationAction> _operationActions = new();
    public IReadOnlyCollection<RoleOperationAction> OperationActions => _operationActions.AsReadOnly();

    private RoleActionType() { }

    public RoleActionType(string code, string nameRu, string nameKz, string nameEn)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Action type code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(nameRu))
            throw new ArgumentException("Russian name is required.", nameof(nameRu));

        Code = code.ToUpperInvariant();
        NameRu = nameRu;
        NameKz = nameKz ?? nameRu;
        NameEn = nameEn ?? nameRu;
    }
}
