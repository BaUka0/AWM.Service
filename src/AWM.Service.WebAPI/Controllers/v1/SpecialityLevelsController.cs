namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.University.Queries.GetSpecialityLevels;
using AWM.Service.WebAPI.Common.Contracts.Responses.University;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/speciality-levels")]
[ApiController]
[Authorize]
public class SpecialityLevelsController : BaseController
{
    private readonly ISender _sender;
    public SpecialityLevelsController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialityLevelResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpecialityLevels(CancellationToken ct)
    {
        var result = await _sender.Send(new GetSpecialityLevelsQuery(), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value.Adapt<IReadOnlyList<SpecialityLevelResponse>>());
    }
}
