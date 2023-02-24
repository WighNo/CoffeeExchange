namespace CoffeeExchange.Data.Context.Entities;

public class ProductInAssortment : EntityBase
{
    public Product Product { get; set; } = null!;

    public decimal Price { get; set; }
    
    public int Count { get; set; }
}