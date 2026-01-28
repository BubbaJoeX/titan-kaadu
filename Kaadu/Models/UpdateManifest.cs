using System.Text.Json.Serialization;

namespace Kaadu.Models;

/// <summary>
/// Represents the remote update manifest containing version and file information.
/// </summary>
public sealed class UpdateManifest
{
    [JsonPropertyName("latest_version")]
    public string LatestVersion { get; set; } = string.Empty;

    [JsonPropertyName("files")]
    public List<FileEntry> Files { get; set; } = [];

    [JsonPropertyName("generated_at")]
    public DateTime GeneratedAt { get; set; }

    [JsonPropertyName("total_size")]
    public long TotalSize { get; set; }
}
