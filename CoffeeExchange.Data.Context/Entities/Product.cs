using CoffeeExchange.Data.Requests.Models;

namespace CoffeeExchange.Data.Context.Entities;

public class Product : EntityBase
{
    public string Name { get; set; } = null!;

    public float MinimalPrice { get; set; }

    public Product()
    {

    }

    public Product(CreateProductRequest request)
    {
        Name = request.Name;
        MinimalPrice = request.MinimalPrice;
    }
}