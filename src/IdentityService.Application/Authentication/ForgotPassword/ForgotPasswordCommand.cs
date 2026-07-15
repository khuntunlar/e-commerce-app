using IdentityService.Application.Common.Responses;
using MediatR;

namespace IdentityService.Application.Authentication.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordDto>;
