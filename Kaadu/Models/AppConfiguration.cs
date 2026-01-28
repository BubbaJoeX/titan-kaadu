using System.IO;
using System.Reflection;

namespace Kaadu.Models;

/// <summary>
/// Application configuration constants.
/// </summary>
public static class AppConfiguration
{
    /// <summary>
    /// The current launcher version.
    /// </summary>
    public const string LauncherVersion = "1.0.3";

    /// <summary>
    /// The default local installation path for the game.
    /// </summary>
    public const string DefaultInstallPath = @"C:\Users\Public\Titan\SWG\";

    private static string _installPath = DefaultInstallPath;

    /// <summary>
    /// The current installation path. Set by SettingsService on startup.
    /// </summary>
    public static string InstallPath
    {
        get => _installPath;
        set => _installPath = string.IsNullOrWhiteSpace(value) ? DefaultInstallPath : value;
    }

    /// <summary>
    /// The authentication endpoint URL.
    /// </summary>
    public const string AuthUrl = "https://swgtitan.org/auth.php";

    /// <summary>
    /// The admin level checker endpoint URL.
    /// </summary>
    public const string AdminCheckerUrl = "https://swgtitan.org/admin_check.php";

    /// <summary>
    /// The game login server address.
    /// </summary>
    public const string GameLoginServerAddress = "test.swgtitan.org";

    /// <summary>
    /// SFTP server hostname.
    /// </summary>
    public const string SftpHost = "test.swgtitan.org";

    /// <summary>
    /// SFTP server port.
    /// </summary>
    public const int SftpPort = 22;

    /// <summary>
    /// Remote path for base game archive.
    /// </summary>
    public const string BaseGamePath = "/upload/base/game.zip";

    /// <summary>
    /// Remote path for update files.
    /// </summary>
    public const string UpdatesPath = "/upload/updates/";

    /// <summary>
    /// Remote path for configuration files.
    /// </summary>
    public const string ConfigPath = "/upload/config/";

    /// <summary>
    /// Remote path for launcher version info.
    /// </summary>
    public const string LauncherVersionPath = "/upload/config/launcher.json";

    /// <summary>
    /// Remote path for launcher binaries (version folders live here).
    /// </summary>
    public const string LauncherBinariesPath = "/upload/launcher/";

    /// <summary>
    /// Name of the update manifest file.
    /// </summary>
    public const string ManifestFileName = "update_manifest.json";

    /// <summary>
    /// Local version tracking file name.
    /// </summary>
    public const string LocalVersionFileName = "version.json";

    /// <summary>
    /// Base installation marker file name.
    /// </summary>
    public const string BaseInstalledMarker = ".base_installed";

    /// <summary>
    /// Gets the full path to the local version file.
    /// </summary>
    public static string LocalVersionPath => Path.Combine(InstallPath, LocalVersionFileName);

    /// <summary>
    /// Gets the full path to the base installed marker file.
    /// </summary>
    public static string BaseInstalledMarkerPath => Path.Combine(InstallPath, BaseInstalledMarker);

    /// <summary>
    /// Gets the path to the current launcher executable.
    /// </summary>
    public static string LauncherExecutablePath => Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location;

    /// <summary>
    /// Gets the directory containing the launcher.
    /// </summary>
    public static string LauncherDirectory => Path.GetDirectoryName(LauncherExecutablePath) ?? AppDomain.CurrentDomain.BaseDirectory;
}
