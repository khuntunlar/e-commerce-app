using MediatR;

namespace IdentityService.Application.Authentication.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword) : IRequest<Unit>;
