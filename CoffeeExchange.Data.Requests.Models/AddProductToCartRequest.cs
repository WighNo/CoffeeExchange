namespace CoffeeExchange.Data.Requests.Models;

public record struct AddProductToCartRequest(int ProductId, int Count);