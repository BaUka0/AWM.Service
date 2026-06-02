namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.University.Queries.GetOrgUnits;
using AWM.Service.Application.Features.University.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class InstitutesController : BaseController
{
    private readonly ISender _sender;
    public InstitutesController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrgUnitDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstitutes(CancellationToken ct)
    {
        var result = await _sender.Send(new GetOrgUnitsQuery(2), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value);
    }
}
