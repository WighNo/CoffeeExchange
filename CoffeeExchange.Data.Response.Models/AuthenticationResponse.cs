namespace CoffeeExchange.Data.Response.Models;

public record struct AuthenticationResponse(string AccessToken, string RefreshToken);