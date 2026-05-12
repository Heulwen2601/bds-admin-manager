using BdsAdmin.API.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BdsAdmin.API.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
