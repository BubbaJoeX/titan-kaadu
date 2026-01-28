using System.IO;
using System.Text.Json;
using Kaadu.Models;

namespace Kaadu.Services;

/// <summary>
/// Implementation of settings service for persisting user preferences.
/// </summary>
public sealed class SettingsService : ISettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Kaadu");

    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private UserSettings _settings = new();

    public UserSettings Settings => _settings;

    public string InstallPath => _settings.EffectiveInstallPath;

    public bool HasConfiguredInstallPath => _settings.HasCustomInstallPath;

    public bool HasRunSetup => _settings.HasRunSetup;

    public bool IsMuted
    {
        get => _settings.IsMuted;
        set => _settings.IsMuted = value;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = await File.ReadAllTextAsync(SettingsFilePath, cancellationToken).ConfigureAwait(false);
                _settings = JsonSerializer.Deserialize<UserSettings>(json, JsonOptions) ?? new UserSettings();
            }
        }
        catch
        {
            // If settings can't be loaded, use defaults
            _settings = new UserSettings();
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);
            var json = JsonSerializer.Serialize(_settings, JsonOptions);
            await File.WriteAllTextAsync(SettingsFilePath, json, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Silently fail on save errors - settings will use defaults next time
        }
    }

    public async Task SetInstallPathAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        _settings.InstallPath = path;
        await SaveAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task MarkSetupCompleteAsync(CancellationToken cancellationToken = default)
    {
        _settings.HasRunSetup = true;
        await SaveAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task ToggleMuteAsync(CancellationToken cancellationToken = default)
    {
        _settings.IsMuted = !_settings.IsMuted;
        await SaveAsync(cancellationToken).ConfigureAwait(false);
    }
}
