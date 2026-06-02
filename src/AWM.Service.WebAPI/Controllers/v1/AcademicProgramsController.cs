namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.University.Queries.GetSpecialities;
using AWM.Service.Application.Features.University.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/academic-programs")]
[ApiController]
[Authorize]
public class AcademicProgramsController : BaseController
{
    private readonly ISender _sender;
    public AcademicProgramsController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPrograms(CancellationToken ct)
    {
        var result = await _sender.Send(new GetSpecialitiesQuery(), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value);
    }
}
