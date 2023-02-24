using Microsoft.AspNetCore.Http;

namespace CoffeeExchange.Data.Response.Errors.NotFound;

public class ProductNotFound : CustomErrorBase
{
    public ProductNotFound(int productId)
    {
        Content = CreateErrorContent($"Товар с ID {productId} не найден");
    }

    public override CustomErrorContent Content { get; }

    public override int StatusCode => StatusCodes.Status404NotFound;
}