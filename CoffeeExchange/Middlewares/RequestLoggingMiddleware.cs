namespace CoffeeExchange.Middlewares;

/// <summary>
/// Middleware для логирования всех запросов
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="next">Следующий middleware</param>
    /// <param name="loggerFactory">Фабрика логгеров</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
    }

    /// <summary>
    /// Шаблонный метод для middleware'ов в ASP
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation(
            "Request {method} {url} => {statusCode}",
            context.Request?.Method,
            context.Request?.Path.Value,
            context.Response?.StatusCode);

        await _next.Invoke(context);
    }
}

/// <summary>
/// Декоратор
/// </summary>
public static class RequestLoggingMiddlewareExtension
{
    /// <summary>
    /// Декоратор подключения логгирования запросов
    /// </summary>
    /// <param name="applicationBuilder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseMiddleware<RequestLoggingMiddleware>();
        return applicationBuilder;
    }
}