using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeExchange.Data.Response.Errors;

public abstract class CustomErrorBase : IActionResult
{
    public abstract CustomErrorContent Content { get; }
    
    public abstract int StatusCode { get; }
    
    private static Dictionary<int, string> StatusCodesTitles => new()
    {
        {StatusCodes.Status400BadRequest, "Bad Request"},
        {StatusCodes.Status404NotFound, "Not Found"}
    };

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(Content)
        {
            StatusCode = StatusCode
        };

        await objectResult.ExecuteResultAsync(context);
    }

    public CustomErrorContent CreateErrorContent(string content)
    {
        return new CustomErrorContent(StatusCodesTitles[StatusCode], content);
    }
}