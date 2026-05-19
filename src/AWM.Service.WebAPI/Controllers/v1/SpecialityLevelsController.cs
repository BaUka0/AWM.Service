namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.DegreeLevels.Queries.GetDegreeLevels;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using AWM.Service.WebAPI.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Controller for managing university speciality levels.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/speciality-levels")]
[Produces("application/json")]
public sealed class SpecialityLevelsController : BaseController
{
    private readonly ISender _sender;

    public SpecialityLevelsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all speciality levels with optional filtering.
    /// </summary>
    [HttpGet]
    [RequireAccess("Specialities", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialityLevelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSpecialityLevels(
        [FromQuery] string? name,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDegreeLevelsQuery
        {
            Name = name
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Select(dto => new SpecialityLevelResponse
        {
            Id = dto.Id,
            Name = dto.Name
        }).ToList();

        return Ok(response);
    }
}
