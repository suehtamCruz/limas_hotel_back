using Microsoft.AspNetCore.Diagnostics;

namespace LimasHotel.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Erro não tratado no endpoint {Method} {Path}.",
            httpContext.Request.Method, httpContext.Request.Path);

        var message = exception.Message;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(
            new { message, status = 500 },
            cancellationToken: cancellationToken);

        return true;
    }
}
