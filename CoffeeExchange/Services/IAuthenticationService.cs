using CoffeeExchange.Data.Requests.Models;

namespace CoffeeExchange.Services;

/// <summary>
/// Контракт для работы с сервисом
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="request">Параметры</param>
    /// <returns></returns>
    public Task<ValueTuple<bool, string>> Register(AuthenticationRequest request);
    
    /// <summary>
    /// Регистрация и задание роли
    /// </summary>
    /// <param name="request">Параметры</param>
    /// <returns></returns>
    public Task<ValueTuple<bool, string>> RegisterWithRole(RegistrationWithRoleRequest request);

    /// <summary>
    /// Получение JWT-токена
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<ValueTuple<bool, string>> Login(AuthenticationRequest request);
}