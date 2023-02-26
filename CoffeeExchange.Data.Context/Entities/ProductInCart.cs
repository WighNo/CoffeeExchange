namespace CoffeeExchange.Data.Context.Entities;

public class ProductInCart : EntityBase
{
    public Product Product { get; set; } = null!;
    
    public int Count { get; set; } 
    
    public bool IsStock { get; set; }
}