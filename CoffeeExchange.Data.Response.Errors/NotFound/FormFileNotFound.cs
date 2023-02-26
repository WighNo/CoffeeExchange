using Microsoft.AspNetCore.Http;

namespace CoffeeExchange.Data.Response.Errors.NotFound;

public class FormFileNotFound : CustomErrorBase
{
    public FormFileNotFound(string formFileKey)
    {
        Content = CreateErrorContent($"Не удалось найти FormFile по ключу {formFileKey}");
    }
    
    public override CustomErrorContent Content { get; }
    
    public override int StatusCode => StatusCodes.Status404NotFound;
}