namespace CoffeeExchange.Data.Requests.Models;

public record struct AddCoffeeHouseRequest(string Name, string Address, double Latitude, double Longitude);