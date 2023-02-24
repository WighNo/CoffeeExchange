using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeExchange.Data.Response.Errors.NotFound;

public class ProductInAssortmentNotFound : CustomErrorBase
{
    public ProductInAssortmentNotFound(int productId)
    {
        Content = CreateErrorContent($"Товар в ассортименте с ID {productId} не найден");
    }

    public override CustomErrorContent Content { get; }

    public override int StatusCode => StatusCodes.Status404NotFound;
}