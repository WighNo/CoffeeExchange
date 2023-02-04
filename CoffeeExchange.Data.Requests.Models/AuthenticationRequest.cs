
namespace CoffeeExchange.Data.Requests.Models;

public record struct AuthenticationRequest(string Login, string Password);

public record struct RegistrationWithRoleRequest(string Login, string Password, string Role);