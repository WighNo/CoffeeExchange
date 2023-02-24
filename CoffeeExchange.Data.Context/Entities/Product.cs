namespace CoffeeExchange.Data.Context.Entities;

public class Product : EntityBase
{
    public string Name { get; set; } = null!;

    public decimal MinimalPrice { get; set; }
}