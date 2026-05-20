namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Role operation (module/resource) with tree structure support.
/// </summary>
public class RoleOperation : Entity<int>, IAuditable
{
    public int? ParentId { get; private set; }
    public string Name { get; private set; } = null!;
    public string NameRu { get; private set; } = null!;
    public string NameKz { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public int OrderBy { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public RoleOperation? Parent { get; private set; }
    private readonly List<RoleOperation> _children = new();
    public IReadOnlyCollection<RoleOperation> Children => _children.AsReadOnly();

    private readonly List<RoleOperationAction> _operationActions = new();
    public IReadOnlyCollection<RoleOperationAction> OperationActions => _operationActions.AsReadOnly();

    private RoleOperation() { }

    public RoleOperation(string name, string nameRu, string nameKz, string nameEn, int createdBy, int? parentId = null, int orderBy = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("RoleOperation.NameRequired", "Operation name is required.");
        if (string.IsNullOrWhiteSpace(nameRu))
            throw new DomainException("RoleOperation.NameRuRequired", "Russian name is required.");

        Name = name;
        NameRu = nameRu;
        NameKz = nameKz ?? nameRu;
        NameEn = nameEn ?? nameRu;
        ParentId = parentId;
        OrderBy = orderBy;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(string name, string nameRu, string nameKz, string nameEn, int orderBy, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("RoleOperation.NameRequired", "Operation name is required.");
        if (string.IsNullOrWhiteSpace(nameRu))
            throw new DomainException("RoleOperation.NameRuRequired", "Russian name is required.");

        Name = name;
        NameRu = nameRu;
        NameKz = nameKz ?? nameRu;
        NameEn = nameEn ?? nameRu;
        OrderBy = orderBy;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void SetParent(int? parentId, int modifiedBy)
    {
        ParentId = parentId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public RoleOperation AddChild(string name, string nameRu, string nameKz, string nameEn, int createdBy, int orderBy = 0)
    {
        var child = new RoleOperation(name, nameRu, nameKz, nameEn, createdBy, this.Id, orderBy);
        _children.Add(child);
        return child;
    }
}
