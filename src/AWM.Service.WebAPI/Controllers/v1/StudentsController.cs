namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.University.Queries.GetStudents;
using AWM.Service.Application.Features.University.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class StudentsController : BaseController
{
    private readonly ISender _sender;
    public StudentsController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents([FromQuery] string? search, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await _sender.Send(new GetStudentsQuery(search, status), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value);
    }
}
