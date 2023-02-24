using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeExchange.Configs;

/// <summary>
/// Конфигурация для аутентификации
/// </summary>
public class AuthenticationConfig
{
    /// <summary>
    /// Название секции в файле "appsettings.json"
    /// </summary>
    public const string SectionKey = "AuthorizationSettings";
    
    /// <summary>
    /// Издатель ключа
    /// </summary>
    public string Issuer { get; set; }  = string.Empty;
    
    /// <summary>
    /// Получатель ключа
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Токен на предъявителя
    /// </summary>
    public string BearerKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Время валидности токена
    /// </summary>
    public int LifeTime { get; set; }
    
    /// <summary>
    /// Получить ключ шифрования
    /// </summary>
    /// <returns></returns>
    public SymmetricSecurityKey SymmetricSecurityKey() => new (Encoding.UTF8.GetBytes(BearerKey));
}