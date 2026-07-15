using MediatR;

namespace IdentityService.Application.Authentication.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string ResetToken, string NewPassword) : IRequest<Unit>;
