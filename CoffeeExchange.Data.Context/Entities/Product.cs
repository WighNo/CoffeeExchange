namespace CoffeeExchange.Data.Context.Entities;

public class Product : EntityBase
{
    public string Name { get; set; } = null!;

    public decimal MinimalPrice { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public string Volume { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Compound { get; set; } = null!;
}