using Kaadu.Models;

namespace Kaadu.Services;

/// <summary>
/// Service for user authentication via HTTPS.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authentication result.</returns>
    Task<AuthResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
}
