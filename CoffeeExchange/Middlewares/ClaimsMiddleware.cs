using System.Security.Claims;
using CoffeeExchange.Middlewares;

namespace CoffeeExchange.Middlewares;

/// <summary>
/// ПО промежуточного слоя для определения claim'ов
/// </summary>
public class ClaimsMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="next"></param>
    public ClaimsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.User.Identity is not null && httpContext.User.Identity.IsAuthenticated)
        {
            var userId = int.Parse(httpContext.User.FindFirst("id")!.Value);
            httpContext.Items["userId"] = userId;
        }

        await _next(httpContext);
    }
}

public static class ClaimsMiddlewareExtensions
{
    public static WebApplication UseClaimsDetermination(this WebApplication application)
    {
        application.UseMiddleware<ClaimsMiddleware>();
        return application;
    }
}