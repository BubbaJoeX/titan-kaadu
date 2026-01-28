namespace Kaadu.Models;

/// <summary>
/// Represents the result of an authentication attempt.
/// </summary>
public sealed record AuthResult(
    bool Success,
    string? ErrorMessage = null,
    UserSession? Session = null);
