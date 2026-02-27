namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Org.Institutes.Commands.CreateInstitute;
using AWM.Service.Application.Features.Org.Institutes.Commands.DeleteInstitute;
using AWM.Service.Application.Features.Org.Institutes.Commands.UpdateInstitute;
using AWM.Service.Application.Features.Org.Institutes.Queries.GetAllInstitutes;
using AWM.Service.Application.Features.Org.Institutes.Queries.GetInstituteById;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Institutes;
using AWM.Service.WebAPI.Common.Contracts.Responses.Org;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Institutes.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class InstitutesController : BaseController
{
    private readonly ISender _sender;

    public InstitutesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all institutes for a specific university.
    /// </summary>
    /// <param name="universityId">University ID</param>
    /// <param name="includeDepartments">Include departments in response</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of institutes</returns>
    [HttpGet]
    [RequirePermission(Permission.Institutes_View)]
    [ProducesResponseType(typeof(IReadOnlyList<InstituteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int universityId,
        [FromQuery] bool includeDepartments = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllInstitutesQuery
        {
            UniversityId = universityId,
            IncludeDepartments = includeDepartments
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value
            .Select(dto => new InstituteResponse
            {
                Id = dto.Id,
                UniversityId = dto.UniversityId,
                Name = dto.Name,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastModifiedAt = dto.LastModifiedAt,
                LastModifiedBy = dto.LastModifiedBy,
                Departments = dto.Departments?.Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    InstituteId = d.InstituteId,
                    Name = d.Name,
                    Code = d.Code,
                    CreatedAt = d.CreatedAt,
                    CreatedBy = d.CreatedBy,
                    LastModifiedAt = d.LastModifiedAt,
                    LastModifiedBy = d.LastModifiedBy
                }).ToList()
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get a specific institute by ID.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <param name="includeDepartments">Include departments in response</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Institute details</returns>
    [HttpGet("{instituteId}")]
    [RequireInstitutePermission(Permission.Institute_Manage)]
    [ProducesResponseType(typeof(InstituteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(
        int instituteId,
        [FromQuery] bool includeDepartments = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInstituteByIdQuery
        {
            InstituteId = instituteId,
            IncludeDepartments = includeDepartments
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var dto = result.Value;
        var response = new InstituteResponse
        {
            Id = dto.Id,
            UniversityId = dto.UniversityId,
            Name = dto.Name,
            CreatedAt = dto.CreatedAt,
            CreatedBy = dto.CreatedBy,
            LastModifiedAt = dto.LastModifiedAt,
            LastModifiedBy = dto.LastModifiedBy,
            Departments = dto.Departments?.Select(d => new DepartmentResponse
            {
                Id = d.Id,
                InstituteId = d.InstituteId,
                Name = d.Name,
                Code = d.Code,
                CreatedAt = d.CreatedAt,
                CreatedBy = d.CreatedBy,
                LastModifiedAt = d.LastModifiedAt,
                LastModifiedBy = d.LastModifiedBy
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new institute.
    /// </summary>
    /// <param name="request">Create institute request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created institute ID</returns>
    [HttpPost]
    [RequirePermission(Permission.Institute_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateInstituteRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateInstituteCommand
        {
            UniversityId = request.UniversityId,
            Name = request.Name
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { instituteId = result.Value, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing institute.
    /// </summary>
    /// <param name="instituteId">Institute ID</param>
    /// <param name="request">Update institute request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPut("{instituteId}")]
    [RequireInstitutePermission(Permission.Institute_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int instituteId, [FromBody] UpdateInstituteRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateInstituteCommand
        {
            InstituteId = instituteId,
            Name = request.Name
        };

        var result = await _sender.Send(command, cancellationToken);

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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{instituteId}")]
    [RequireInstitutePermission(Permission.Institute_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int instituteId, CancellationToken cancellationToken = default)
    {
        var command = new DeleteInstituteCommand
        {
            InstituteId = instituteId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
