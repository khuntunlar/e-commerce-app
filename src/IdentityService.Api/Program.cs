using IdentityService.Api.Middleware;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Authentication.AssignRole;
using IdentityService.Application.Authentication.ChangePassword;
using IdentityService.Application.Authentication.ForgotPassword;
using IdentityService.Application.Authentication.Login;
using IdentityService.Application.Authentication.Logout;
using IdentityService.Application.Authentication.Refresh;
using IdentityService.Application.Authentication.Register;
using IdentityService.Application.Authentication.ResetPassword;
using IdentityService.Application.Common.Behaviors;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Security;
using IdentityService.Infrastructure.Time;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"] ?? string.Empty;
var identityConnectionString = builder.Configuration.GetConnectionString("Identity") ?? string.Empty;
if (!builder.Environment.IsDevelopment()
    && (string.IsNullOrWhiteSpace(jwtSigningKey) || string.IsNullOrWhiteSpace(identityConnectionString)))
{
    throw new InvalidOperationException("Identity connection string and Jwt:SigningKey must be configured outside development.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:3000"];

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddIdentityPersistence(builder.Configuration);
builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();
builder.Services.AddScoped<IValidator<LoginUserCommand>, LoginUserCommandValidator>();
builder.Services.AddScoped<IValidator<RefreshSessionCommand>, RefreshSessionCommandValidator>();
builder.Services.AddScoped<IValidator<LogoutSessionCommand>, LogoutSessionCommandValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordCommand>, ChangePasswordCommandValidator>();
builder.Services.AddScoped<IValidator<AssignRoleCommand>, AssignRoleCommandValidator>();
builder.Services.AddScoped<IValidator<ForgotPasswordCommand>, ForgotPasswordCommandValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("auth-sensitive", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var signingKey = builder.Configuration["Jwt:SigningKey"] ?? "7Xlcw53AYw6VmPpz0etfkX41+qiBJ4t14ZLQ5D/mSu1kQC0dc7rOt+JbMhOW/1Um";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Container"))
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("Frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
