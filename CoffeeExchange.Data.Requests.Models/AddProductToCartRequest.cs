using System.ComponentModel.DataAnnotations;

namespace CoffeeExchange.Data.Requests.Models;

public record AddProductToCartRequest
{
    [Required] public int ProductId { get; set; }
    
    [Required] public int Count { get; set; }
}