namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.AcademicPrograms.Queries.GetAcademicPrograms;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using AWM.Service.WebAPI.Authorization;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing academic programs.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/academic-programs")]
[Produces("application/json")]
public sealed class AcademicProgramsController : BaseController
{
    private readonly ISender _sender;

    public AcademicProgramsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get academic programs with optional filtering.
    /// </summary>
    /// <param name="departmentId">Filter by department ID (optional).</param>
    /// <param name="degreeLevelId">Filter by degree level ID (optional).</param>
    /// <param name="code">Filter by program code (optional, partial match).</param>
    /// <param name="name">Filter by program name (optional, partial match).</param>
    /// <param name="includeDeleted">Include soft-deleted programs (default: false).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of academic programs.</returns>
    [HttpGet]
    [RequireAccess("AcademicPrograms", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<AcademicProgramResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAcademicPrograms(
        [FromQuery] int? departmentId,
        [FromQuery] int? degreeLevelId,
        [FromQuery] string? code,
        [FromQuery] string? name,
        [FromQuery] bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAcademicProgramsQuery
        {
            DepartmentId = departmentId,
            DegreeLevelId = degreeLevelId,
            Code = code,
            Name = name,
            IncludeDeleted = includeDeleted
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<AcademicProgramResponse>>();

        return Ok(response);
    }
}