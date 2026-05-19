namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Org.Institutes.Queries.GetAllInstitutes;
using AWM.Service.Application.Features.Org.Institutes.Queries.GetInstituteById;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses.Org;
using Mapster;
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
    /// Get all institutes.
    /// </summary>
    /// <param name="includeDepartments">Include departments in response</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of institutes</returns>
    [HttpGet]
    [RequireAccess("Org_Institutes", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<InstituteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeDepartments = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllInstitutesQuery
        {
            IncludeDepartments = includeDepartments
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<InstituteResponse>>();

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
    [RequireAccess("Org_Institutes", "Read")]
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

        var response = result.Value.Adapt<InstituteResponse>();

        return Ok(response);
    }
}
