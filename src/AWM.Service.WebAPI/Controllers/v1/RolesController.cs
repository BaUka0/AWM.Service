namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleAccesses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AWM.Service.WebAPI.Common.Contracts.Responses.Auth;
using Mapster;

using System.Linq;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class RolesController : BaseController
{
    private readonly ISender _sender;
    public RolesController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllRoleAccessesQuery(), ct);
        var dtos = result.Adapt<IReadOnlyList<RoleAccessResponse>>();
        return Ok(dtos);
    }
}

