using System.ComponentModel.DataAnnotations;

namespace CoffeeExchange.Data.Requests.Models;

public record AddCoffeeHouseRequest
{
    [Required] public string Name { get; set; } = null!;

    [Required] public string Address { get; set; } = null!;

    [Required] public double Latitude { get; set; }

    [Required] public double Longitude { get; set; }
}