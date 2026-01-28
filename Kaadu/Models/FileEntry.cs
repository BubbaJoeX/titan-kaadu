using System.Text.Json.Serialization;

namespace Kaadu.Models;

/// <summary>
/// Represents a file entry in the update manifest with path and checksum.
/// </summary>
public sealed class FileEntry
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("checksum")]
    public string Checksum { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }
}
