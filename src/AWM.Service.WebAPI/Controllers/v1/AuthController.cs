using AWM.Service.Application.Features.Auth.Commands.Login;
using AWM.Service.WebAPI.Common.Contracts.Requests;
using AWM.Service.WebAPI.Common.Contracts.Responses;
using AWM.Service.WebAPI.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for authentication operations.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : BaseController
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _sender.Send(new LoginCommand(request.Login, request.Password));
        
        if (result.IsSuccess)
        {
            var authResult = result.Value;
            var response = new LoginResponse(
                Token: authResult.Token,
                Login: authResult.Login,
                UserId: authResult.UserId,
                Email: authResult.Email,
                Roles: authResult.Roles
            );
            return Ok(response);
        }
        
        return HandleResultError(result.Error);
    }
}
