using IdentityService.Application.Common.Requests;
using IdentityService.Application.Authentication.Login;
using IdentityService.Application.Authentication.Logout;
using IdentityService.Application.Authentication.Me;
using IdentityService.Application.Authentication.Refresh;
using IdentityService.Application.Authentication.Register;
using IdentityService.Application.Common;
using IdentityService.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthSessionDto>> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterUserCommand(request.Email, request.Password, request.DisplayName),
            cancellationToken);

        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthSessionDto>> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LoginUserCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthSessionDto>> Refresh([FromBody] RefreshSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshSessionCommand(request.RefreshToken), cancellationToken);
        return Ok(result);
    }

    [HttpPost("change-password")]
    public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        => StatusCode(StatusCodes.Status501NotImplemented, new ProblemDetails
        {
            Title = "Authentication scaffold",
            Detail = "Change password endpoint will be implemented in the next slice."
        });

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshSessionRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LogoutSessionCommand(request.RefreshToken), cancellationToken);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthenticatedUser>> Me(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return Ok(result);
    }
}
