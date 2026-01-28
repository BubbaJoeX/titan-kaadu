using System.IO;
using System.Security.Cryptography;

namespace Kaadu.Services;

/// <summary>
/// Implementation of hash service using SHA-256.
/// </summary>
public sealed class HashService : IHashService
{
    public async Task<string> ComputeSha256Async(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found for hash computation.", filePath);
        }

        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        var hashBytes = await SHA256.HashDataAsync(stream, cancellationToken).ConfigureAwait(false);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public async Task<bool> VerifyHashAsync(string filePath, string expectedHash, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(expectedHash);

        if (!File.Exists(filePath))
        {
            return false;
        }

        var actualHash = await ComputeSha256Async(filePath, cancellationToken).ConfigureAwait(false);
        return string.Equals(actualHash, expectedHash.ToLowerInvariant(), StringComparison.Ordinal);
    }
}
