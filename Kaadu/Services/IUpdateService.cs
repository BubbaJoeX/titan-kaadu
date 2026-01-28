using Kaadu.Models;

namespace Kaadu.Services;

/// <summary>
/// Service for managing game updates and installations.
/// </summary>
public interface IUpdateService
{
    /// <summary>
    /// Gets the current update status.
    /// </summary>
    UpdateStatus Status { get; }

    /// <summary>
    /// Gets the current status message.
    /// </summary>
    string StatusMessage { get; }

    /// <summary>
    /// Gets the current download progress (0-100).
    /// </summary>
    double Progress { get; }

    /// <summary>
    /// Occurs when status changes.
    /// </summary>
    event EventHandler<UpdateStatus>? StatusChanged;

    /// <summary>
    /// Occurs when progress changes.
    /// </summary>
    event EventHandler<double>? ProgressChanged;

    /// <summary>
    /// Occurs when status message changes.
    /// </summary>
    event EventHandler<string>? StatusMessageChanged;

    /// <summary>
    /// Checks if the base game is installed.
    /// </summary>
    bool IsBaseGameInstalled { get; }

    /// <summary>
    /// Gets the locally installed version.
    /// </summary>
    Task<string> GetLocalVersionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the remote update manifest.
    /// </summary>
    Task<UpdateManifest?> GetRemoteManifestAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs full update check and applies any needed updates.
    /// </summary>
    Task<bool> CheckAndUpdateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads and installs the base game.
    /// </summary>
    Task<bool> InstallBaseGameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies all game files and re-downloads any corrupted files.
    /// </summary>
    Task<bool> RepairGameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Launches the game executable.
    /// </summary>
    Task LaunchGameAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens the SwgTitanSetup_r.exe for initial configuration.
    /// </summary>
    Task OpenSetupExecutableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Launches the game with auto-login parameters.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="serverAddress">The login server address.</param>
    /// <param name="selectedCharacter">The selected character name (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task LaunchGameWithAutoLoginAsync(string username, string password, string serverAddress, string? selectedCharacter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans the remote /updates/ folder, generates a manifest, and uploads it to /config/.
    /// Admin-only operation.
    /// </summary>
    /// <param name="version">The version string for the manifest.</param>
    /// <param name="progress">Progress callback for status updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GenerateAndUploadManifestAsync(string version, IProgress<string>? progress = null, CancellationToken cancellationToken = default);
}
