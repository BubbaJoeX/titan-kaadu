using System.Text.Json.Serialization;

namespace Kaadu.Models;

/// <summary>
/// User settings that persist between application sessions.
/// </summary>
public sealed class UserSettings
{
    /// <summary>
    /// The custom installation path for the game.
    /// Null indicates the default path should be used.
    /// </summary>
    [JsonPropertyName("installPath")]
    public string? InstallPath { get; set; }

    /// <summary>
    /// Gets whether a custom install path has been configured.
    /// </summary>
    [JsonIgnore]
    public bool HasCustomInstallPath => !string.IsNullOrWhiteSpace(InstallPath);

    /// <summary>
    /// Gets the effective install path (custom or default).
    /// </summary>
    [JsonIgnore]
    public string EffectiveInstallPath => HasCustomInstallPath 
        ? InstallPath! 
        : AppConfiguration.DefaultInstallPath;

    /// <summary>
    /// Indicates whether the SwgClientSetup_r.exe has been run at least once.
    /// </summary>
    [JsonPropertyName("hasRunSetup")]
    public bool HasRunSetup { get; set; }

    /// <summary>
    /// Indicates whether the background audio is muted.
    /// </summary>
    [JsonPropertyName("isMuted")]
    public bool IsMuted { get; set; }
}
