namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Commands.AcademicPrograms.CreateAcademicProgram;
using AWM.Service.Application.Features.Edu.Commands.AcademicPrograms.UpdateAcademicProgram;
using AWM.Service.Application.Features.Edu.Queries.AcademicPrograms.GetAcademicPrograms;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing academic programs.
/// </summary>
[ApiController]
[Route("api/v1/academic-programs")]
[Produces("application/json")]
public sealed class AcademicProgramsController : ControllerBase
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

        var response = result.Value
            .Select(dto => new AcademicProgramResponse
            {
                Id = dto.Id,
                DepartmentId = dto.DepartmentId,
                DegreeLevelId = dto.DegreeLevelId,
                Code = dto.Code,
                Name = dto.Name,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastModifiedAt = dto.LastModifiedAt,
                LastModifiedBy = dto.LastModifiedBy,
                IsDeleted = dto.IsDeleted,
                DeletedAt = dto.DeletedAt,
                DeletedBy = dto.DeletedBy
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Create a new academic program.
    /// </summary>
    /// <param name="request">Academic program creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created program ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAcademicProgram(
        [FromBody] CreateAcademicProgramRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new CreateAcademicProgramCommand
        {
            DepartmentId = request.DepartmentId,
            DegreeLevelId = request.DegreeLevelId,
            Code = request.Code,
            Name = request.Name,
            CreatedBy = currentUserId
        };

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAcademicProgram(
        [FromRoute] int id,
        [FromBody] UpdateAcademicProgramRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from HttpContext/Claims
        var currentUserId = GetCurrentUserId();

        var command = new UpdateAcademicProgramCommand
        {
            Id = id,
            Code = request.Code,
            Name = request.Name,
            ModifiedBy = currentUserId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Handles Result errors and converts them to appropriate HTTP responses.
    /// </summary>
    private IActionResult HandleResultError(KDS.Primitives.FluentResult.Error error)
    {
        return error.Code switch
        {
            "404" or "AcademicProgram.NotFound" or "Department.NotFound" or "DegreeLevel.NotFound" 
                => NotFound(new { error.Code, error.Message }),
            
            "Validation.Error" or "AcademicProgram.Deleted" 
                => BadRequest(new { error.Code, error.Message }),
            
            "Query.NotSupported" 
                => BadRequest(new { error.Code, error.Message }),
            
            _ => StatusCode(StatusCodes.Status500InternalServerError, 
                new { error.Code, error.Message })
        };
    }

    /// <summary>
    /// Gets current user ID from authentication context.
    /// TODO: Implement actual user resolution from JWT/Claims.
    /// </summary>
    private int GetCurrentUserId()
    {
        // Placeholder: Extract from User.Claims or ICurrentUserService
        // Example: return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return 1; // Mock user ID for now
    }
}