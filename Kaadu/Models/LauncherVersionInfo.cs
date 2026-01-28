using System.Text.Json.Serialization;

namespace Kaadu.Models;

/// <summary>
/// Represents the launcher version information from the update server.
/// </summary>
public sealed record LauncherVersionInfo
{
    /// <summary>
    /// The current launcher version (e.g., "1.0.2").
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Optional release notes or changelog.
    /// </summary>
    [JsonPropertyName("releaseNotes")]
    public string? ReleaseNotes { get; init; }

    /// <summary>
    /// Optional minimum required version - forces update if local version is below this.
    /// </summary>
    [JsonPropertyName("minimumVersion")]
    public string? MinimumVersion { get; init; }

    /// <summary>
    /// The filename of the launcher executable in the version folder.
    /// </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; init; } = "Kaadu.exe";
}
