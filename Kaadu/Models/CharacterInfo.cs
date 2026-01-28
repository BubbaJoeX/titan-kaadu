namespace Kaadu.Models;

/// <summary>
/// Represents a character from the character list file.
/// </summary>
public sealed record CharacterInfo(
    string Server,
    string CharacterName,
    string Field3,
    string Field4)
{
    /// <summary>
    /// Gets the display name for the character (includes server suffix).
    /// </summary>
    public string DisplayName => CharacterName;

    /// <summary>
    /// Gets the base character name without server suffix (e.g., "BubbaJoe" instead of "BubbaJoe (Titan)").
    /// </summary>
    public string BaseName
    {
        get
        {
            var parenIndex = CharacterName.IndexOf(" (", StringComparison.Ordinal);
            return parenIndex > 0 ? CharacterName[..parenIndex] : CharacterName;
        }
    }
}
