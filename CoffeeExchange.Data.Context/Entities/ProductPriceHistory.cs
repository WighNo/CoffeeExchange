namespace CoffeeExchange.Data.Context.Entities;

public class ProductPriceHistory : EntityBase
{
    public CoffeeHouse CoffeeHouse { get; set; } = null!;
    
    public Product Product { get; set; } = null!;
    
    public decimal Price { get; set; }
    
    public DateTime Date { get; set; }
}