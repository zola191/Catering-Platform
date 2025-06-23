using Catering.Platform.Applications.Abstractions;
using System.Security.Cryptography;

namespace Catering.Platform.Applications.Services;

public class HashService : IHashService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    private readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password,salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        var (hash, salt) = ExtractHashAndSalt(passwordHash);
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }

    private static (byte[] hash, byte[] salt) ExtractHashAndSalt(string passwordHash)
    {
        var parts = passwordHash.Split('-');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid password hash format.");

        return (Convert.FromHexString(parts[0]), Convert.FromHexString(parts[1]));
    }
}
