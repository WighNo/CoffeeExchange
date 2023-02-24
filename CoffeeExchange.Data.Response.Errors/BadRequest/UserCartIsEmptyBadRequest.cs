using Microsoft.AspNetCore.Http;

namespace CoffeeExchange.Data.Response.Errors.BadRequest;

public class UserCartIsEmptyBadRequest : CustomErrorBase
{
    public UserCartIsEmptyBadRequest(int userId)
    {
        Content = CreateErrorContent($"Корзина пользователя с ID {userId} пуста");
    }
    
    public override CustomErrorContent Content { get; }

    public override int StatusCode => StatusCodes.Status400BadRequest;
}