using AWM.Service.Application.Features.Org.Commands.Departments.CreateDepartment;
using AWM.Service.Application.Features.Org.Commands.Departments.UpdateDepartment;
using AWM.Service.Application.Features.Org.Queries.Departments.GetDepartmentsByInstitute;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing Departments.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class DepartmentsController : ControllerBase
{
    private readonly ISender _sender;

    public DepartmentsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get all departments for a specific institute.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <returns>List of departments</returns>
    [HttpGet]
    [Route("/api/v1/institutes/{instituteId}/departments")]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Org.DTOs.DepartmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByInstituteId(int instituteId)
    {
        var query = new GetDepartmentsByInstituteQuery
        {
            InstituteId = instituteId
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new department.
    /// </summary>
    /// <param name="request">Create department request</param>
    /// <returns>Created department ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var command = new CreateDepartmentCommand
        {
            InstituteId = request.InstituteId,
            Name = request.Name,
            Code = request.Code,
            CreatedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return CreatedAtAction(
            nameof(GetByInstituteId), 
            new { instituteId = request.InstituteId }, 
            result.Value);
    }

    /// <summary>
    /// Update an existing department.
    /// </summary>
    /// <param name="id">Department ID</param>
    /// <param name="request">Update department request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequest request)
    {
        var command = new UpdateDepartmentCommand
        {
            DepartmentId = id,
            Name = request.Name,
            Code = request.Code,
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
    /// Soft delete a department.
    /// </summary>
    /// <param name="id">Department ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   
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

}

#region Request Models

/// <summary>
/// Request model for creating a department.
/// </summary>
public record CreateDepartmentRequest
{
    public int InstituteId { get; init; }
    public string Name { get; init; } = null!;
    public string? Code { get; init; }
}

/// <summary>
/// Request model for updating a department.
/// </summary>
public record UpdateDepartmentRequest
{
    public string Name { get; init; } = null!;
    public string? Code { get; init; }
}

#endregion