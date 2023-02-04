namespace CoffeeExchange.Data.Requests.Models;

public record struct CreateProductRequest(string Name, float MinimalPrice);