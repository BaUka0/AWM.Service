namespace AWM.Service.Domain.Common;

/// <summary>
/// Interface to provide the current user identifier and university (tenant).
/// </summary>
public interface ICurrentUserProvider
{
    int? UserId { get; }
    int? UniversityId { get; }
    bool IsAuthenticated { get; }
}
