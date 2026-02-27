namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.CreateAcademicProgram;
using AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.UpdateAcademicProgram;
using AWM.Service.Application.Features.Edu.AcademicPrograms.Queries.GetAcademicPrograms;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using AWM.Service.Domain.Auth.Enums;
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
    [RequirePermission(Permission.AcademicPrograms_View)]
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

    /// <summary>
    /// Create a new academic program.
    /// </summary>
    /// <param name="request">Academic program creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created program ID.</returns>
    [HttpPost]
    [RequirePermission(Permission.AcademicPrograms_Create)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAcademicProgram(
        [FromBody] CreateAcademicProgramRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateAcademicProgramCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(
            nameof(GetAcademicPrograms),
            new { departmentId = request.DepartmentId },
            result.Value);
    }

    /// <summary>
    /// Update an existing academic program.
    /// </summary>
    /// <param name="id">Academic program ID.</param>
    /// <param name="request">Updated program data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [RequirePermission(Permission.AcademicPrograms_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAcademicProgram(
        [FromRoute] int id,
        [FromBody] UpdateAcademicProgramRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UpdateAcademicProgramCommand>() with { Id = id };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}