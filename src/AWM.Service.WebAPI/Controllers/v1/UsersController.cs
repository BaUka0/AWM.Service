using AWM.Service.Application.Features.Admin.Users.Commands.CreateUser;
using AWM.Service.Application.Features.Admin.Users.Commands.ToggleUserStatus;
using AWM.Service.Application.Features.Admin.Users.Commands.UpdateUser;
using AWM.Service.Application.Features.Admin.Users.Queries.GetAllUsers;
using AWM.Service.Application.Features.Admin.Users.Queries.GetUserById;
using AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Users;
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
    [HttpGet]
    [RequirePermission(Permission.Users_View)]
    [ProducesResponseType(typeof(IReadOnlyList<AdminUserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int universityId,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery
        {
            UniversityId = universityId,
            IsActive = isActive,
            Search = search
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<AdminUserResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Get a user by ID.
    /// </summary>
    [HttpGet("{userId}")]
    [RequirePermission(Permission.Users_View)]
    [ProducesResponseType(typeof(AdminUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int userId, CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery { UserId = userId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Adapt<AdminUserResponse>();
        return Ok(response);
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    [HttpPost]
    [RequirePermission(Permission.Users_Create)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateUserCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { userId = result.Value, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing user.
    /// </summary>
    [HttpPut("{userId}")]
    [RequirePermission(Permission.Users_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UpdateUserCommand>() with { UserId = userId };
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Toggle user activation status.
    /// </summary>
    [HttpPatch("{userId}/status")]
    [RequirePermission(Permission.Users_Deactivate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ToggleStatus(int userId, [FromBody] ToggleUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ToggleUserStatusCommand { UserId = userId, IsActive = request.IsActive };
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        return NoContent();
    }
}
