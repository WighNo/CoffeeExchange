using Microsoft.AspNetCore.Http;

namespace CoffeeExchange.Data.Response.Errors.NotFound;

public class UserNotFound : CustomErrorBase
{
    public UserNotFound(int userId)
    {
        Content = CreateErrorContent($"Пользователь с ID {userId} не найден");
    }
    
    public override CustomErrorContent Content { get; }

    public override int StatusCode => StatusCodes.Status404NotFound;
}