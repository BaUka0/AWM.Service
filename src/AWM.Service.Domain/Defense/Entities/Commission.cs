namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;

/// <summary>
/// Commission entity - defense commission (PreDefense or GAK).
/// </summary>
public class Commission : AggregateRoot<int>, IAuditable, ISoftDeletable
{
    public int OrgUnitId { get; private set; }
    public int SemesterId { get; private set; }
    public string? Name { get; private set; }
    public int CommissionTypeId { get; private set; }
    public int? PreDefenseNumber { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<CommissionMember> _members = new();
    public IReadOnlyCollection<CommissionMember> Members => _members.AsReadOnly();

    private Commission() { }

    public Commission(
        int orgUnitId,
        int semesterId,
        int commissionTypeId,
        int createdBy,
        string? name = null,
        int? preDefenseNumber = null)
    {
        if (commissionTypeId == (int)CommissionTypes.PreDefense && preDefenseNumber.HasValue)
        {
            if (preDefenseNumber < 1 || preDefenseNumber > 3)
                throw new DomainException("Commission.InvalidPreDefenseNumber", "Pre-defense number must be 1, 2, or 3.");
        }

        OrgUnitId = orgUnitId;
        SemesterId = semesterId;
        CommissionTypeId = commissionTypeId;
        Name = name ?? GetDefaultName(commissionTypeId, preDefenseNumber);
        PreDefenseNumber = preDefenseNumber;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    private static string GetDefaultName(int typeId, int? preDefenseNumber)
    {
        if (typeId == (int)CommissionTypes.PreDefense)
            return $"Комиссия предзащиты №{preDefenseNumber ?? 1}";
        if (typeId == (int)CommissionTypes.GAK)
            return "Государственная аттестационная комиссия";
        
        return "Комиссия";
    }

    /// <summary>
    /// Adds a member to the commission.
    /// </summary>
    public CommissionMember AddMember(int userId, int commissionRoleId)
    {
        // Ensure only one chairman and one secretary
        if (commissionRoleId == (int)CommissionRoles.Chairman && _members.Any(m => m.CommissionRoleId == (int)CommissionRoles.Chairman))
            throw new DomainException("Commission.ChairmanAlreadyExists", "Commission already has a chairman.");

        if (commissionRoleId == (int)CommissionRoles.Secretary && _members.Any(m => m.CommissionRoleId == (int)CommissionRoles.Secretary))
            throw new DomainException("Commission.SecretaryAlreadyExists", "Commission already has a secretary.");

        var member = new CommissionMember(Id, userId, commissionRoleId);
        _members.Add(member);

        LastModifiedAt = DateTime.UtcNow;
        return member;
    }

    /// <summary>
    /// Gets the chairman of the commission.
    /// </summary>
    public CommissionMember? GetChairman()
    {
        return _members.FirstOrDefault(m => m.CommissionRoleId == (int)CommissionRoles.Chairman);
    }

    /// <summary>
    /// Gets the secretary of the commission.
    /// </summary>
    public CommissionMember? GetSecretary()
    {
        return _members.FirstOrDefault(m => m.CommissionRoleId == (int)CommissionRoles.Secretary);
    }

    /// <summary>
    /// Updates commission name.
    /// </summary>
    public void UpdateName(string name, int modifiedBy)
    {
        Name = name;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Removes a member from the commission by member ID.
    /// </summary>
    /// <returns>True if the member was found and removed; otherwise, false.</returns>
    public bool RemoveMember(int memberId, int modifiedBy)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId);
        if (member is null)
            return false;

        _members.Remove(member);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
        return true;
    }

    /// <summary>
    /// Soft deletes the commission.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
