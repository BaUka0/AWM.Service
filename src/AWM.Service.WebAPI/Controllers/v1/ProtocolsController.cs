using AWM.Service.Application.Features.Defense.Protocols.Commands.CreateProtocol;
using AWM.Service.Application.Features.Defense.Protocols.Commands.FinalizeProtocol;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing defense protocols (final decisions and session records).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/protocols")]
[ApiController]
[Authorize]
public sealed class ProtocolsController : BaseController
{
    private readonly ISender _sender;

    public ProtocolsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new protocol for a defense session.
    /// </summary>
    [HttpPost]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> CreateProtocol(
        [FromBody] CreateProtocolRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateProtocolCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Finalizes a protocol, locking all grades and decisions.
    /// </summary>
    [HttpPost("{id}/finalize")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> FinalizeProtocol(
        long id,
        CancellationToken cancellationToken)
    {
        var command = new FinalizeProtocolCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }
}
