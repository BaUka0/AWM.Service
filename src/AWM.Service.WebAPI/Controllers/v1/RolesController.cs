using AWM.Service.Application.Features.Admin.Roles.Queries.GetAllRoles;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing system roles.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class RolesController : BaseController
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all system roles with user counts for a specific university.
    /// </summary>
    [HttpGet]
    [RequireAccess("Users_Roles", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<AdminRoleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var query = new GetAllRolesQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed) return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<AdminRoleResponse>>();
        return Ok(response);
    }
}
