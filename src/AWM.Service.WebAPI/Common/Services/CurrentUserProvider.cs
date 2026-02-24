using System.Security.Claims;
using AWM.Service.Domain.Common;
using AWM.Service.WebAPI.Authorization;

namespace AWM.Service.WebAPI.Common.Services;

/// <summary>
/// Provides current user information from HttpContext for audit and multi-tenancy.
/// </summary>
public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : null;
        }
    }

    public int? UniversityId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var universityClaim = user?.FindFirst(AuthorizationConstants.UniversityIdClaimType)
                               ?? user?.FindFirst("UniversityId");
            return universityClaim != null && int.TryParse(universityClaim.Value, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
