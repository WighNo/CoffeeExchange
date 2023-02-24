namespace CoffeeExchange.Data.Context.Entities;

public class DealRecord : EntityBase 
{
    public float AmountPerUnit { get; set; }

    public int Count { get; set; }
    
    public Product Target { get; set; } = null!;

    public User Owner { get; set; } = null!;
    
    public DateTime Date { get; set; }
}