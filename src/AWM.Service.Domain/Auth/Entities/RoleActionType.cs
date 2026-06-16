namespace AWM.Service.Domain.Auth.Entities;

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
            throw new DomainException("RoleActionType.CodeRequired", "Action type code is required.");
        if (string.IsNullOrWhiteSpace(nameRu))
            throw new DomainException("RoleActionType.NameRuRequired", "Russian name is required.");

        Code = code.ToUpperInvariant();
        NameRu = nameRu;
        NameKz = nameKz ?? nameRu;
        NameEn = nameEn ?? nameRu;
    }
}
