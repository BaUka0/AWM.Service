namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;

/// <summary>
/// Commission entity - defense commission (PreDefense or GAK).
/// </summary>
public class Commission : AggregateRoot<int>, IAuditable, ISoftDeletable
{
    public int OrgUnitId { get; private set; }
    public int? SpecialityId { get; private set; }
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

    private readonly List<StaffAssignment> _assignments = new();
    public IReadOnlyCollection<StaffAssignment> Assignments => _assignments.AsReadOnly();

    private Commission() { }

    public Commission(
        int orgUnitId,
        int semesterId,
        int commissionTypeId,
        int createdBy,
        string? name = null,
        int? preDefenseNumber = null,
        int? specialityId = null)
    {
        if (commissionTypeId == (int)CommissionTypes.PreDefense && preDefenseNumber.HasValue)
        {
            if (preDefenseNumber < 1 || preDefenseNumber > 3)
                throw new DomainException("Commission.InvalidPreDefenseNumber", "Pre-defense number must be 1, 2, or 3.");
        }

        OrgUnitId = orgUnitId;
        SpecialityId = specialityId;
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
    /// Adds a member to the commission using unified staff assignments.
    /// </summary>
    public StaffAssignment AddMember(int userId, StaffRoleType roleType, int createdBy)
    {
        if (roleType == StaffRoleType.CommissionChairman && _assignments.Any(a => a.RoleType == StaffRoleType.CommissionChairman && a.IsActive))
            throw new DomainException("Commission.ChairmanAlreadyExists", "Commission already has an active chairman.");

        if (roleType == StaffRoleType.CommissionSecretary && _assignments.Any(a => a.RoleType == StaffRoleType.CommissionSecretary && a.IsActive))
            throw new DomainException("Commission.SecretaryAlreadyExists", "Commission already has an active secretary.");

        if (CommissionTypeId == (int)CommissionTypes.GAK && roleType == StaffRoleType.CommissionMember)
        {
            var currentMemberCount = _assignments.Count(a => a.RoleType == StaffRoleType.CommissionMember && a.IsActive);
            if (currentMemberCount >= 4)
                throw new DomainException("Commission.TooManyMembers", "GAK commission can have a maximum of 4 members.");
        }

        var assignment = new StaffAssignment(
            userId,
            roleType,
            "Commission",
            Id,
            createdBy);

        _assignments.Add(assignment);

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = createdBy;
        return assignment;
    }

    /// <summary>
    /// Gets the chairman of the commission.
    /// </summary>
    public StaffAssignment? GetChairman()
    {
        return _assignments.FirstOrDefault(a => a.RoleType == StaffRoleType.CommissionChairman && a.IsActive);
    }

    /// <summary>
    /// Gets the secretary of the commission.
    /// </summary>
    public StaffAssignment? GetSecretary()
    {
        return _assignments.FirstOrDefault(a => a.RoleType == StaffRoleType.CommissionSecretary && a.IsActive);
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
    /// Updates commission type and pre-defense number.
    /// </summary>
    public void UpdateCommissionType(int commissionTypeId, int? preDefenseNumber, int modifiedBy)
    {
        if (commissionTypeId == (int)CommissionTypes.PreDefense && preDefenseNumber.HasValue)
        {
            if (preDefenseNumber < 1 || preDefenseNumber > 3)
                throw new DomainException("Commission.InvalidPreDefenseNumber", "Pre-defense number must be 1, 2, or 3.");
        }

        CommissionTypeId = commissionTypeId;
        PreDefenseNumber = preDefenseNumber;
        Name = GetDefaultName(commissionTypeId, preDefenseNumber);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Updates commission speciality.
    /// </summary>
    public void UpdateSpeciality(int? specialityId, int modifiedBy)
    {
        SpecialityId = specialityId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Replaces all active members with a new set (deactivates old, adds new).
    /// Raises domain event for notification tracking.
    /// </summary>
    public void ReplaceMembers(int chairmanUserId, int secretaryUserId, List<int> memberUserIds, int modifiedBy)
    {
        var oldMemberIds = _assignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .Select(a => a.UserId)
            .ToList();

        foreach (var assignment in _assignments.Where(a => a.IsActive && !a.IsDeleted).ToList())
        {
            assignment.Deactivate(modifiedBy);
        }

        AddMember(chairmanUserId, StaffRoleType.CommissionChairman, modifiedBy);
        AddMember(secretaryUserId, StaffRoleType.CommissionSecretary, modifiedBy);

        foreach (var memberId in memberUserIds.Distinct())
        {
            AddMember(memberId, StaffRoleType.CommissionMember, modifiedBy);
        }

        var newMemberIds = new List<int> { chairmanUserId, secretaryUserId };
        newMemberIds.AddRange(memberUserIds);

        var addedUserIds = newMemberIds.Distinct().Except(oldMemberIds).ToList();
        var removedUserIds = oldMemberIds.Except(newMemberIds.Distinct()).ToList();

        if (addedUserIds.Any() || removedUserIds.Any())
        {
            RaiseDomainEvent(new AWM.Service.Domain.Defense.Events.CommissionMembersChangedEvent(
                Id,
                Name ?? GetDefaultName(CommissionTypeId, PreDefenseNumber),
                addedUserIds,
                removedUserIds,
                modifiedBy));
        }

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Removes a member from the commission (deactivates assignment).
    /// </summary>
    public bool RemoveMember(long assignmentId, int modifiedBy)
    {
        var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId);
        if (assignment is null)
            return false;

        assignment.Deactivate(modifiedBy);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
        return true;
    }

    /// <summary>
    /// Validates that the commission has a valid composition:
    /// - Exactly 1 Chairman
    /// - Exactly 1 Secretary
    /// - At least 1 Member
    /// - For GAK, between 1 and 4 Members.
    /// </summary>
    public void ValidateIntegrity()
    {
        var activeAssignments = _assignments.Where(a => a.IsActive && !a.IsDeleted).ToList();

        if (!activeAssignments.Any(a => a.RoleType == StaffRoleType.CommissionChairman))
            throw new DomainException("Commission.MissingChairman", "Commission must have an active chairman.");

        if (!activeAssignments.Any(a => a.RoleType == StaffRoleType.CommissionSecretary))
            throw new DomainException("Commission.MissingSecretary", "Commission must have an active secretary.");

        var memberCount = activeAssignments.Count(a => a.RoleType == StaffRoleType.CommissionMember);

        if (CommissionTypeId == (int)CommissionTypes.GAK)
        {
            if (memberCount < 1 || memberCount > 4)
                throw new DomainException("Commission.InvalidGakMembersCount", "GAK commission must have between 1 and 4 active members.");
        }
        else
        {
            if (memberCount < 1)
                throw new DomainException("Commission.MissingMembers", "Pre-defense commission must have at least one active member.");
        }
    }

    /// <summary>
    /// Soft deletes the commission.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;

        foreach (var assignment in _assignments)
        {
            assignment.Delete(deletedBy);
        }
    }
}
