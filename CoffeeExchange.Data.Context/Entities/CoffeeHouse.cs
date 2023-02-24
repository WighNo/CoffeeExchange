using System.Text.Json.Serialization;

namespace CoffeeExchange.Data.Context.Entities;

public class CoffeeHouse : EntityBase
{
    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public List<ProductInAssortment> Assortment { get; set; } = null!;

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
}