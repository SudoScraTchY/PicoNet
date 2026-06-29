// PicoNet.Infrastructure/Security/PasswordHasher.cs

using System.Security.Cryptography;
using System.Text;

namespace PicoNet.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    // Tuned for URL passwords: fast verification, still secure enough
    private const int SaltSize = 16;        // 128 bits — sufficient with unique salts
    private const int HashSize = 16;        // 128 bits — not storing nuclear codes
    private const int Iterations = 10_000;  // ~5-10ms per hash on modern CPU
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            Algorithm,
            HashSize);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        var parts = hashedPassword.Split(':', 2);
        if (parts.Length != 2) return false;

        byte[] salt, storedHash;
        try
        {
            salt = Convert.FromBase64String(parts[0]);
            storedHash = Convert.FromBase64String(parts[1]);
        }
        catch { return false; }

        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            Algorithm,
            storedHash.Length);

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}