namespace CoffeeExchange.Helpers;

/// <summary>
/// 
/// </summary>
public static class ControlClaimsHelper
{
    private const string UserIdClaimName = "userId";

    /// <summary>
    /// Достаёт из списка claim'ов u
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static int GetUserIdClaim(this HttpContext httpContext)
    {
        var claimValue = httpContext.Items[UserIdClaimName]!;
        return (int) claimValue;
    }
}