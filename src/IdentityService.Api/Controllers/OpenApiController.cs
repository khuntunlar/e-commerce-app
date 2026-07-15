using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
public sealed class OpenApiController : ControllerBase
{
    [HttpGet("openapi/v1.json")]
    public IActionResult GetDocument()
    {
        var document = new
        {
            openapi = "3.0.1",
            info = new
            {
                title = "Identity Service API",
                version = "v1",
                description = "Phase-1 authentication API for the ecommerce platform."
            },
            servers = new[] { new { url = "/" } },
            paths = new Dictionary<string, object>
            {
                ["/api/v1/auth/register"] = Post("Register a new customer", "Create a user and return an authenticated session.", "AuthSession", false),
                ["/api/v1/auth/login"] = Post("Login", "Authenticate a user and return an authenticated session.", "AuthSession", false),
                ["/api/v1/auth/refresh"] = Post("Refresh session", "Exchange a valid refresh token for a new session.", "AuthSession", false),
                ["/api/v1/auth/logout"] = Post("Logout", "Revoke an active refresh token.", null, false, 204),
                ["/api/v1/auth/change-password"] = Post("Change password", "Change the current user's password.", null, true, 204),
                ["/api/v1/auth/forgot-password"] = Post("Forgot password", "Request a password reset token for an account.", null, false, 204),
                ["/api/v1/auth/reset-password"] = Post("Reset password", "Reset a password with a valid reset token.", null, false, 204),
                ["/api/v1/auth/roles/assign"] = Post("Assign role", "Assign a role to a user. Admin role required.", null, true, 204),
                ["/api/v1/auth/me"] = new
                {
                    get = new
                    {
                        tags = new[] { "Authentication" },
                        summary = "Current user profile",
                        security = BearerSecurity(),
                        responses = Responses("AuthenticatedUser")
                    }
                },
                ["/api/v1/health"] = new
                {
                    get = new
                    {
                        tags = new[] { "Health" },
                        summary = "Health check",
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new { description = "Service is healthy" }
                        }
                    }
                }
            },
            components = new
            {
                securitySchemes = new Dictionary<string, object>
                {
                    ["Bearer"] = new
                    {
                        type = "http",
                        scheme = "bearer",
                        bearerFormat = "JWT"
                    }
                },
                schemas = Schemas()
            }
        };

        return Ok(document);
    }

    [HttpGet("swagger")]
    public ContentResult GetSwaggerUi()
    {
        const string html = """
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <title>Identity Service API</title>
  <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
</head>
<body>
  <div id="swagger-ui"></div>
  <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
  <script>SwaggerUIBundle({ url: '/openapi/v1.json', dom_id: '#swagger-ui' });</script>
</body>
</html>
""";

        return Content(html, "text/html");
    }

    private static object Post(string summary, string description, string? successSchema, bool authorize, int successStatus = 200)
    {
        var operation = new Dictionary<string, object>
        {
            ["tags"] = new[] { "Authentication" },
            ["summary"] = summary,
            ["description"] = description,
            ["responses"] = Responses(successSchema, successStatus)
        };

        if (authorize)
        {
            operation["security"] = BearerSecurity();
        }

        return new Dictionary<string, object> { ["post"] = operation };
    }

    private static Dictionary<string, object>[] BearerSecurity() =>
        new[] { new Dictionary<string, object> { ["Bearer"] = Array.Empty<string>() } };

    private static Dictionary<string, object> Responses(string? successSchema, int successStatus = 200)
    {
        var responses = new Dictionary<string, object>
        {
            [successStatus.ToString()] = successSchema is null
                ? new { description = "Success" }
                : new
                {
                    description = "Success",
                    content = new Dictionary<string, object>
                    {
                        ["application/json"] = new
                        {
                            schema = new Dictionary<string, object>
                            {
                                ["$ref"] = $"#/components/schemas/{successSchema}"
                            }
                        }
                    }
                },
            ["400"] = Problem("Validation failed"),
            ["401"] = Problem("Authentication failed"),
            ["403"] = Problem("Forbidden"),
            ["409"] = Problem("Conflict"),
            ["500"] = Problem("Unexpected error")
        };

        return responses;
    }

    private static object Problem(string description) => new
    {
        description,
        content = new Dictionary<string, object>
        {
            ["application/problem+json"] = new
            {
                schema = new Dictionary<string, object>
                {
                    ["$ref"] = "#/components/schemas/ProblemDetails"
                }
            }
        }
    };

    private static Dictionary<string, object> Schemas() => new()
    {
        ["AuthSession"] = new
        {
            type = "object",
            properties = new Dictionary<string, object>
            {
                ["accessToken"] = new { type = "string" },
                ["refreshToken"] = new { type = "string" },
                ["expiresIn"] = new { type = "integer", format = "int32" },
                ["user"] = new Dictionary<string, object> { ["$ref"] = "#/components/schemas/AuthenticatedUser" }
            }
        },
        ["AuthenticatedUser"] = new
        {
            type = "object",
            properties = new Dictionary<string, object>
            {
                ["id"] = new { type = "string", format = "uuid" },
                ["email"] = new { type = "string", format = "email" },
                ["displayName"] = new { type = "string" },
                ["roles"] = new { type = "array", items = new { type = "string" } }
            }
        },
        ["ProblemDetails"] = new
        {
            type = "object",
            properties = new Dictionary<string, object>
            {
                ["title"] = new { type = "string" },
                ["status"] = new { type = "integer", format = "int32" },
                ["detail"] = new { type = "string" }
            }
        }
    };
}
