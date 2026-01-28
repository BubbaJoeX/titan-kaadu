namespace Kaadu.Models;

/// <summary>
/// Represents an authenticated user session with permission level.
/// </summary>
public sealed record UserSession(
    string Username,
    bool IsAuthenticated,
    int AdminLevel,
    string? SessionToken = null,
    string? Password = null,
    string? ServerAddress = null)
{
    /// <summary>
    /// Gets whether the user has admin privileges.
    /// </summary>
    public bool IsAdmin => AdminLevel > 0;

    /// <summary>
    /// Creates an unauthenticated session.
    /// </summary>
    public static UserSession Unauthenticated => new(string.Empty, false, 0);
}
