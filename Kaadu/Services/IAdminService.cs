namespace Kaadu.Services;

/// <summary>
/// Service for checking admin permissions.
/// </summary>
public interface IAdminService
{
    /// <summary>
    /// Gets the admin level for a user.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The admin level (0 = no admin, higher = more privileges).</returns>
    Task<int> GetAdminLevelAsync(string username, CancellationToken cancellationToken = default);
}
