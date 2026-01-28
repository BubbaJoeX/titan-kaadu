using Kaadu.Models;

namespace Kaadu.Services;

/// <summary>
/// Service for managing character lists.
/// </summary>
public interface ICharacterService
{
    /// <summary>
    /// Gets the list of characters for the specified username.
    /// </summary>
    /// <param name="username">The username to get characters for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of characters, or empty list if no character file exists.</returns>
    Task<List<CharacterInfo>> GetCharactersAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a character list file exists for the specified username.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the character list file exists.</returns>
    bool HasCharacterList(string username);
}
