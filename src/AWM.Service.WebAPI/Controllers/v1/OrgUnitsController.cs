namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.University.Queries.GetOrgUnits;
using AWM.Service.WebAPI.Common.Contracts.Responses.University;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class OrgUnitsController : BaseController
{
    private readonly ISender _sender;
    public OrgUnitsController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrgUnitResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrgUnits([FromQuery] int typeId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetOrgUnitsQuery(typeId), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value.Adapt<IReadOnlyList<OrgUnitResponse>>());
    }
}
