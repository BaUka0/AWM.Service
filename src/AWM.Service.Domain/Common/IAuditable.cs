namespace AWM.Service.Domain.Common;

/// <summary>
/// Base interface for auditable entities that track creation and modification.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; }
    int CreatedBy { get; }
    DateTime? LastModifiedAt { get; }
    int? LastModifiedBy { get; }
}

/// <summary>
/// Base interface for soft-deletable entities.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    int? DeletedBy { get; }
}
