using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeExchange.Configs;

public class AuthenticationConfig
{
    public const string SectionKey = "AuthorizationSettings";
    
    public string Issuer { get; set; }  = string.Empty;
    
    public string Audience { get; set; } = string.Empty;
    
    public string BearerKey { get; set; } = string.Empty;
    
    public int LifeTime { get; set; }
    
    public SymmetricSecurityKey SymmetricSecurityKey() => new (Encoding.UTF8.GetBytes(BearerKey));
}