namespace Kaadu.Models;

/// <summary>
/// Represents the current update/launcher status.
/// </summary>
public enum UpdateStatus
{
    /// <summary>
    /// Initial state, not yet checked.
    /// </summary>
    Idle,

    /// <summary>
    /// Checking for updates.
    /// </summary>
    Checking,

    /// <summary>
    /// Downloading base game or updates.
    /// </summary>
    Downloading,

    /// <summary>
    /// Extracting downloaded archives.
    /// </summary>
    Extracting,

    /// <summary>
    /// Verifying file integrity.
    /// </summary>
    Verifying,

    /// <summary>
    /// Ready to play, all files verified.
    /// </summary>
    Ready,

    /// <summary>
    /// An error occurred during the update process.
    /// </summary>
    Error
}
