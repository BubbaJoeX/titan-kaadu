namespace Kaadu.Services;

/// <summary>
/// Service for computing file hashes.
/// </summary>
public interface IHashService
{
    /// <summary>
    /// Computes the SHA-256 hash of a file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The hash as a lowercase hex string.</returns>
    Task<string> ComputeSha256Async(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a file's hash matches the expected value.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="expectedHash">The expected hash (lowercase hex).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if hash matches.</returns>
    Task<bool> VerifyHashAsync(string filePath, string expectedHash, CancellationToken cancellationToken = default);
}
