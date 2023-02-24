using System.Security.Cryptography;
using CoffeeExchange.Data.Context.Entities;

namespace CoffeeExchange.Helpers;

/// <summary>
/// Методы-расширения для аутентификации
/// </summary>
public static class AuthenticationHelpers
{
    /// <summary>
    /// Вычисление соли
    /// </summary>
    /// <param name="user">Пользователь</param>
    public static void ProvideSaltAndHash(this User user)
    {
        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        Span<byte> salt = stackalloc byte[24];
        randomNumberGenerator.GetBytes(salt);
        
        user.Salt = Convert.ToBase64String(salt);
        user.PasswordHash = ComputeHash(user.PasswordHash, user.Salt);
    }
        
    /// <summary>
    /// Вычисление хеша пароля
    /// </summary>
    /// <param name="password">Пароль</param>
    /// <param name="userSalt">Соль</param>
    /// <returns></returns>
    public static string ComputeHash(string password, string userSalt)
    {
        byte[] salt = Convert.FromBase64String(userSalt);

        using var hashGenerator = new Rfc2898DeriveBytes(password, salt, 10101, HashAlgorithmName.SHA256);
        byte[] bytes = hashGenerator.GetBytes(24);
        
        return Convert.ToBase64String(bytes);
    }
}