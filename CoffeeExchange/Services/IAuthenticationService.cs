using CoffeeExchange.Data.Requests.Models;

namespace CoffeeExchange.Services;

public interface IAuthenticationService
{
    public Task<ValueTuple<bool, string>> Register(AuthenticationRequest request);
    
    public Task<ValueTuple<bool, string>> RegisterWithRole(RegistrationWithRoleRequest request);

    public Task<ValueTuple<bool, string>> Login(AuthenticationRequest request);
}