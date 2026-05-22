using AWM.Service.Application.Features.Admin.Users.Queries.GetAllUsers;
using AWM.Service.Application.Features.Admin.Users.Queries.GetUserById;
using AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for user management and profile operations.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class UsersController : BaseController
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get the full profile of the currently authenticated user.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { Code = "401", Message = "Не удалось определить пользователя из токена." });
        }

        var query = new GetCurrentUserProfileQuery { UserId = userId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<UserProfileResponse>();
        return Ok(response);
    }

    /// <summary>
    /// Get all users with optional filters.
    /// </summary>
    /// <param name="page">Current page number (1-based).</param>
    /// <param name="pageSize">Number of users per page (default: 10).</param>
    /// <param name="isActive">Optional filter by active status.</param>
    /// <param name="search">Optional search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [RequireAccess("Users", "Read")]
    [ProducesResponseType(typeof(PagedResponse<AdminUserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery
        {
            IsActive = isActive,
            Search = search,
            Page = page,
            PageSize = pageSize
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        var (items, totalCount) = result.Value;

        var response = new PagedResponse<AdminUserResponse>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items.Adapt<IReadOnlyList<AdminUserResponse>>()
        };

        return Ok(response);
    }

    /// <summary>
    /// Get a user by ID.
    /// </summary>
    [HttpGet("{userId}")]
    [RequireAccess("Users", "Read")]
    [ProducesResponseType(typeof(AdminUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int userId, CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery { UserId = userId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Adapt<AdminUserResponse>();
        return Ok(response);
    }
}
