namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.University.Queries.GetSpecialityLevels;
using AWM.Service.WebAPI.Common.Contracts.Responses.University;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/degree-levels")]
[ApiController]
[Authorize]
public class DegreeLevelsController : BaseController
{
    private readonly ISender _sender;
    public DegreeLevelsController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DegreeLevelResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLevels(CancellationToken ct)
    {
        var result = await _sender.Send(new GetSpecialityLevelsQuery(), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value.Adapt<IReadOnlyList<DegreeLevelResponse>>());
    }
}

