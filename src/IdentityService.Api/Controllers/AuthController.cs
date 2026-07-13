using IdentityService.Application.Common.Requests;
using IdentityService.Application.Authentication.AssignRole;
using IdentityService.Application.Authentication.ChangePassword;
using IdentityService.Application.Authentication.ForgotPassword;
using IdentityService.Application.Authentication.Login;
using IdentityService.Application.Authentication.Logout;
using IdentityService.Application.Authentication.Me;
using IdentityService.Application.Authentication.Refresh;
using IdentityService.Application.Authentication.Register;
using IdentityService.Application.Authentication.ResetPassword;
using IdentityService.Application.Common;
using IdentityService.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
    [EnableRateLimiting("auth-sensitive")]
    public async Task<ActionResult<AuthSessionDto>> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LoginUserCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth-sensitive")]
    public async Task<ActionResult<AuthSessionDto>> Refresh([FromBody] RefreshSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshSessionCommand(request.RefreshToken), cancellationToken);
        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ChangePasswordCommand(request.CurrentPassword, request.NewPassword),
            cancellationToken);

        return NoContent();
    }

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

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth-sensitive")]
    public async Task<ActionResult<ForgotPasswordDto>> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("auth-sensitive")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ResetPasswordCommand(request.Email, request.ResetToken, request.NewPassword),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("roles/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new AssignRoleCommand(request.UserId, request.RoleName), cancellationToken);
        return NoContent();
    }
}
