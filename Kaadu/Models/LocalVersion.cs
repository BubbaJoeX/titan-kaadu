using System.Text.Json.Serialization;

namespace Kaadu.Models;

/// <summary>
/// Represents the local version tracking file stored at the install path.
/// </summary>
public sealed class LocalVersion
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "0.0.0";

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;

    [JsonPropertyName("base_installed")]
    public bool BaseInstalled { get; set; }
}
