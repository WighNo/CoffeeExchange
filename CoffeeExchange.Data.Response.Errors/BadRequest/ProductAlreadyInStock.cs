using Microsoft.AspNetCore.Http;

namespace CoffeeExchange.Data.Response.Errors.BadRequest;

public class ProductAlreadyInStock : CustomErrorBase
{
    public ProductAlreadyInStock(int productId, int coffeeHouseId)
    {
        Content = CreateErrorContent($"Товар с ID {productId} уже добавлен в кофейню с ID {coffeeHouseId}");
    }
    
    public override CustomErrorContent Content { get; }
    
    public override int StatusCode => StatusCodes.Status404NotFound;
}