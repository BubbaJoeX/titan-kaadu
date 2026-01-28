using System.IO;
using Kaadu.Models;

namespace Kaadu.Services;

/// <summary>
/// Implementation of character list service.
/// </summary>
public sealed class CharacterService : ICharacterService
{
    public async Task<List<CharacterInfo>> GetCharactersAsync(string username, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(username);

        var characterFilePath = GetCharacterFilePath(username);
        
        if (!File.Exists(characterFilePath))
        {
            return new List<CharacterInfo>();
        }

        try
        {
            var characters = new List<CharacterInfo>();
            var lines = await File.ReadAllLinesAsync(characterFilePath, cancellationToken).ConfigureAwait(false);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');
                if (parts.Length >= 2)
                {
                    var server = parts[0].Trim();
                    var characterName = parts[1].Trim();
                    var field3 = parts.Length > 2 ? parts[2].Trim() : string.Empty;
                    var field4 = parts.Length > 3 ? parts[3].Trim() : string.Empty;

                    characters.Add(new CharacterInfo(server, characterName, field3, field4));
                }
            }

            return characters;
        }
        catch (IOException)
        {
            // File system error - return empty list
            return new List<CharacterInfo>();
        }
        catch (UnauthorizedAccessException)
        {
            // Access denied - return empty list
            return new List<CharacterInfo>();
        }
        catch (Exception)
        {
            // Unexpected error parsing character list - return empty list
            return new List<CharacterInfo>();
        }
    }

    public bool HasCharacterList(string username)
    {
        ArgumentNullException.ThrowIfNull(username);
        return File.Exists(GetCharacterFilePath(username));
    }

    private static string GetCharacterFilePath(string username)
    {
        return Path.Combine(AppConfiguration.InstallPath, $"characterlist_{username}.txt");
    }
}
