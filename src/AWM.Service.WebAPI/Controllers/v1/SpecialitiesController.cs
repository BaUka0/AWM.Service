namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Specialities.Queries.GetSpecialities;
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
/// Controller for managing university specialities.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/specialities")]
[Produces("application/json")]
public sealed class SpecialitiesController : BaseController
{
    private readonly ISender _sender;

    public SpecialitiesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get specialities with optional filtering.
    /// </summary>
    [HttpGet]
    [RequireAccess("Specialities", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSpecialities(
        [FromQuery] int? degreeLevelId,
        [FromQuery] string? code,
        [FromQuery] string? name,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSpecialitiesQuery
        {
            DegreeLevelId = degreeLevelId,
            Code = code,
            Name = name
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Select(dto => new SpecialityResponse       
        {
            Id = dto.Id,
            Code = dto.Code ?? string.Empty,
            Name = dto.Name ?? string.Empty,
            LevelId = dto.DegreeLevelId,
            IsDeleted = dto.IsDeleted
        }).ToList();

        return Ok(response);
    }
}
