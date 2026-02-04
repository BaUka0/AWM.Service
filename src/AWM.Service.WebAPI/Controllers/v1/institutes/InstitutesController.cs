using AWM.Service.Application.Features.Org.Commands.Institutes.CreateInstitute;
using AWM.Service.Application.Features.Org.Commands.Institutes.DeleteInstitute;
using AWM.Service.Application.Features.Org.Commands.Institutes.UpdateInstitute;
using AWM.Service.Application.Features.Org.Queries.Institutes.GetAllInstitutes;
using AWM.Service.Application.Features.Org.Queries.Institutes.GetInstituteById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1.institutes;

/// <summary>
/// Controller for managing Institutes.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class InstitutesController : ControllerBase
{
    private readonly ISender _sender;

    public InstitutesController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get all institutes for a specific university.
    /// </summary>
    /// <param name="universityId">University ID</param>
    /// <param name="includeDepartments">Include departments in response</param>
    /// <returns>List of institutes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Org.DTOs.InstituteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] int universityId, [FromQuery] bool includeDepartments = false)
    {
        var query = new GetAllInstitutesQuery
        {
            UniversityId = universityId,
            IncludeDepartments = includeDepartments
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a specific institute by ID.
    /// </summary>
    /// <param name="id">Institute ID</param>
    /// <param name="includeDepartments">Include departments in response</param>
    /// <returns>Institute details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Application.Features.Org.DTOs.InstituteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool includeDepartments = false)
    {
        var query = new GetInstituteByIdQuery
        {
            InstituteId = id,
            IncludeDepartments = includeDepartments
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new institute.
    /// </summary>
    /// <param name="request">Create institute request</param>
    /// <returns>Created institute ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateInstituteRequest request)
    {
        var command = new CreateInstituteCommand
        {
            UniversityId = request.UniversityId,
            Name = request.Name,
            CreatedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update an existing institute.
    /// </summary>
    /// <param name="id">Institute ID</param>
    /// <param name="request">Update institute request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInstituteRequest request)
    {
        var command = new UpdateInstituteCommand
        {
            InstituteId = id,
            Name = request.Name,
            ModifiedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete an institute.
    /// </summary>
    /// <param name="id">Institute ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteInstituteCommand
        {
            InstituteId = id,
            DeletedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return NoContent();
    }

    #region Helper Methods

    /// <summary>
    /// Handles errors from FluentResult and returns appropriate HTTP status codes.
    /// </summary>
    private IActionResult HandleError(KDS.Primitives.FluentResult.Error error)
    {
        return error.Code switch
        {
            var code when code.StartsWith("NotFound") => NotFound(new { error.Code, error.Message }),
            var code when code.StartsWith("Validation") => BadRequest(new { error.Code, error.Message }),
            var code when code.StartsWith("BusinessRule") => Conflict(new { error.Code, error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Code, error.Message })
        };
    }

    /// <summary>
    /// Gets current user ID from authentication context.
    /// TODO: Implement actual user context retrieval (e.g., from JWT claims).
    /// </summary>
    private int GetCurrentUserId()
    {
        // TODO: Extract from HttpContext.User.Claims or similar
        return 1; // Placeholder
    }

    #endregion
}

#region Request Models

/// <summary>
/// Request model for creating an institute.
/// </summary>
public record CreateInstituteRequest
{
    public int UniversityId { get; init; }
    public string Name { get; init; } = null!;
}

/// <summary>
/// Request model for updating an institute.
/// </summary>
public record UpdateInstituteRequest
{
    public string Name { get; init; } = null!;
}

#endregion