using Kaadu.Models;

namespace Kaadu.Services;

/// <summary>
/// Service for checking and applying launcher updates.
/// </summary>
public interface ILauncherUpdateService
{
    /// <summary>
    /// Gets the current local launcher version.
    /// </summary>
    string LocalVersion { get; }

    /// <summary>
    /// Checks if a launcher update is available.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The remote version info if an update is available, null otherwise.</returns>
    Task<LauncherVersionInfo?> CheckForUpdateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads and applies the launcher update.
    /// </summary>
    /// <param name="versionInfo">The version info to download.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if update was downloaded successfully and restart is needed.</returns>
    Task<bool> DownloadAndApplyUpdateAsync(LauncherVersionInfo versionInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs when download progress changes.
    /// </summary>
    event EventHandler<double>? ProgressChanged;

    /// <summary>
    /// Occurs when status message changes.
    /// </summary>
    event EventHandler<string>? StatusChanged;
}
