using System.Security.Cryptography;
using CoffeeExchange.Data.Context.Entities;

namespace CoffeeExchange.Helpers;

public static class AuthenticationHelpers
{
    public static void ProvideSaltAndHash(this User user)
    {
        byte[] salt = GenerateSalt();
        user.Salt = Convert.ToBase64String(salt);
        user.PasswordHash = ComputeHash(user.PasswordHash, user.Salt);
    }
        
    private static byte[] GenerateSalt()
    {
        RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        byte[] salt = new byte[24];
        randomNumberGenerator.GetBytes(salt);
            
        return salt;
    }

    public static string ComputeHash(string password, string userSalt)
    {
        byte[] salt = Convert.FromBase64String(userSalt);

        using var hashGenerator = new Rfc2898DeriveBytes(password, salt, 10101, HashAlgorithmName.SHA256);
        byte[] bytes = hashGenerator.GetBytes(24);
        
        return Convert.ToBase64String(bytes);
    }
}