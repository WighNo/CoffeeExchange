namespace CoffeeExchange.Data.Context.Entities;

public class ProductSalesHistory : EntityBase
{
    public Product Product { get; set; } = null!;
    
    public decimal Price { get; set; }
    
    public DateTime Date { get; set; }
}