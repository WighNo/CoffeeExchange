using System.ComponentModel.DataAnnotations;

namespace CoffeeExchange.Data.Requests.Models;

public record CreateProductRequest
{
    [Required] public string Name { get; set; } = null!;

    [Required] public float MinimalPrice { get; set; }

    [Required] public string FormImageKey { get; set; } = null!;
    
    [Required] public string Volume { get; set; } = null!;

    [Required] public string Description { get; set; } = null!;

    [Required] public string Compound { get; set; } = null!;
}