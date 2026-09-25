using System.Security.Cryptography;
using System.Text;

namespace PCChangeTracker.Windows;

public sealed class ComparisonKey : IDisposable
{
    private readonly byte[] key;
    public string Id { get; }

    public ComparisonKey(string dataDirectory, byte[]? material = null)
    {
        if (material is not null)
        {
            if (material.Length != 32) throw new CryptographicException("The comparison key length is invalid.");
            key = material.ToArray();
            Id = Convert.ToHexString(SHA256.HashData(key));
            return;
        }
        Directory.CreateDirectory(dataDirectory);
        var path = Path.Combine(dataDirectory, "comparison-key.bin");
        if (!File.Exists(path))
        {
            var generated = RandomNumberGenerator.GetBytes(32);
            try
            {
                var encrypted = ProtectedData.Protect(generated, null, DataProtectionScope.CurrentUser);
                using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                stream.Write(encrypted);
                stream.Flush(true);
            }
            finally { CryptographicOperations.ZeroMemory(generated); }
        }
        key = ProtectedData.Unprotect(File.ReadAllBytes(path), null, DataProtectionScope.CurrentUser);
        if (key.Length != 32) throw new CryptographicException("The local comparison key is invalid.");
        Id = Convert.ToHexString(SHA256.HashData(key));
    }

    public string Fingerprint(string value) => Convert.ToHexString(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(value)));
    public void Dispose() => CryptographicOperations.ZeroMemory(key);
}