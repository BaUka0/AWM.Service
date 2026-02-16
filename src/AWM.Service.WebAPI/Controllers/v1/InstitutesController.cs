using AWM.Service.Application.Features.Org.Commands.Institutes.CreateInstitute;
using AWM.Service.Application.Features.Org.Commands.Institutes.DeleteInstitute;
using AWM.Service.Application.Features.Org.Commands.Institutes.UpdateInstitute;
using AWM.Service.Application.Features.Org.Queries.Institutes.GetAllInstitutes;
using AWM.Service.Application.Features.Org.Queries.Institutes.GetInstituteById;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Institutes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing Institutes.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class InstitutesController : BaseController
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
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a specific institute by ID.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <param name="includeDepartments">Include departments in response</param>
    /// <returns>Institute details</returns>
    [HttpGet("{instituteId}")]
    [RequireInstitutePermission(Permission.Institute_Manage)]
    [ProducesResponseType(typeof(Application.Features.Org.DTOs.InstituteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int instituteId, [FromQuery] bool includeDepartments = false)
    {
        var query = new GetInstituteByIdQuery
        {
            InstituteId = instituteId,
            IncludeDepartments = includeDepartments
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new institute.
    /// </summary>
    /// <param name="request">Create institute request</param>
    /// <returns>Created institute ID</returns>
    [HttpPost]
    [RequirePermission(Permission.Institute_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateInstituteRequest request)
    {
        var command = new CreateInstituteCommand
        {
            UniversityId = request.UniversityId,
            Name = request.Name
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing institute.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <param name="request">Update institute request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{instituteId}")]
    [RequireInstitutePermission(Permission.Institute_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int instituteId, [FromBody] UpdateInstituteRequest request)
    {
        var command = new UpdateInstituteCommand
        {
            InstituteId = instituteId,
            Name = request.Name
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete an institute.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{instituteId}")]
    [RequireInstitutePermission(Permission.Institute_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int instituteId)
    {
        var command = new DeleteInstituteCommand
        {
            InstituteId = instituteId
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}

