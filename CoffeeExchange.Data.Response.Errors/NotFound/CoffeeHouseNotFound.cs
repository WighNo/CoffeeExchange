using Microsoft.AspNetCore.Http;

namespace CoffeeExchange.Data.Response.Errors.NotFound;

public class CoffeeHouseNotFound : CustomErrorBase
{
    public CoffeeHouseNotFound(int coffeeHouseId)
    {
        Content = CreateErrorContent($"Кофейня с ID {coffeeHouseId} не найдена");
    }

    public override CustomErrorContent Content { get; }

    public override int StatusCode => StatusCodes.Status404NotFound;
}