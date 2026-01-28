using System.Text.Json;
using Renci.SshNet;

namespace Kaadu.Deploy;

/// <summary>
/// Deployment tool for uploading launcher updates to the SFTP server.
/// 
/// Usage: Kaadu.Deploy.exe &lt;version&gt; &lt;launcher-exe-path&gt; [--host &lt;host&gt;] [--user &lt;user&gt;] [--password &lt;password&gt;]
/// 
/// Example: Kaadu.Deploy.exe 1.0.2 ./bin/Release/net9.0-windows/publish/Kaadu.exe
/// 
/// Environment variables (alternative to command-line args):
///   SFTP_HOST     - SFTP server hostname (default: test.swgtitan.org)
///   SFTP_USER     - SFTP username
///   SFTP_PASSWORD - SFTP password
/// </summary>
public static class Program
{
    private const string DefaultHost = "test.swgtitan.org";
    private const int DefaultPort = 22;
    private const string RemoteBasePath = "/upload";
    private const string LauncherPath = "/launcher";
    private const string ConfigPath = "/config";

    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=== Kaadu Launcher Deployment Tool ===\n");

        if (args.Length < 2)
        {
            PrintUsage();
            return 1;
        }

        var version = args[0].TrimStart('v', 'V');
        var exePath = args[1];

        // Validate version format
        if (!Version.TryParse(version, out _))
        {
            Console.WriteLine($"Error: Invalid version format '{version}'. Expected format: X.Y.Z (e.g., 1.0.2)");
            return 1;
        }

        // Validate exe path
        if (!File.Exists(exePath))
        {
            Console.WriteLine($"Error: Launcher executable not found: {exePath}");
            return 1;
        }

        // Parse optional arguments
        var host = GetArgValue(args, "--host") ?? Environment.GetEnvironmentVariable("SFTP_HOST") ?? DefaultHost;
        var user = GetArgValue(args, "--user") ?? Environment.GetEnvironmentVariable("SFTP_USER");
        var password = GetArgValue(args, "--password") ?? Environment.GetEnvironmentVariable("SFTP_PASSWORD");

        if (string.IsNullOrEmpty(user))
        {
            Console.Write("SFTP Username: ");
            user = Console.ReadLine();
        }

        if (string.IsNullOrEmpty(password))
        {
            Console.Write("SFTP Password: ");
            password = ReadPassword();
            Console.WriteLine();
        }

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Error: Username and password are required.");
            return 1;
        }

        try
        {
            await DeployAsync(host, user, password, version, exePath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
            return 1;
        }
    }

    private static async Task DeployAsync(string host, string user, string password, string version, string exePath)
    {
        Console.WriteLine($"Connecting to {host}...");

        using var client = new SftpClient(host, DefaultPort, user, password);
        client.Connect();

        Console.WriteLine("Connected!\n");

        // Get the publish directory (parent of the exe)
        var publishDir = Path.GetDirectoryName(exePath) ?? throw new InvalidOperationException("Cannot determine publish directory");
        var remoteLauncherDir = $"{RemoteBasePath}{LauncherPath}/v{version}";
        var remoteConfigPath = $"{RemoteBasePath}{ConfigPath}/launcher.json";

        // Create version directory if it doesn't exist
        Console.WriteLine($"Creating directory: {remoteLauncherDir}");
        CreateDirectoryRecursive(client, remoteLauncherDir);

        // Get all files to upload
        var filesToUpload = Directory.GetFiles(publishDir, "*", SearchOption.AllDirectories);
        Console.WriteLine($"Found {filesToUpload.Length} files to upload\n");

        var uploadedCount = 0;
        var totalSize = filesToUpload.Sum(f => new FileInfo(f).Length);
        long uploadedSize = 0;

        foreach (var localFile in filesToUpload)
        {
            var relativePath = Path.GetRelativePath(publishDir, localFile).Replace('\\', '/');
            var remotePath = $"{remoteLauncherDir}/{relativePath}";
            var remoteDir = Path.GetDirectoryName(remotePath)?.Replace('\\', '/');

            // Create subdirectory if needed
            if (!string.IsNullOrEmpty(remoteDir) && remoteDir != remoteLauncherDir)
            {
                CreateDirectoryRecursive(client, remoteDir);
            }

            var fileInfo = new FileInfo(localFile);
            Console.Write($"[{uploadedCount + 1}/{filesToUpload.Length}] {relativePath} ({FormatSize(fileInfo.Length)})... ");

            await using (var fileStream = File.OpenRead(localFile))
            {
                client.UploadFile(fileStream, remotePath, true);
            }

            uploadedSize += fileInfo.Length;
            uploadedCount++;
            Console.WriteLine("OK");
        }

        Console.WriteLine($"\nUploaded {uploadedCount} files ({FormatSize(uploadedSize)})");

        // Update launcher.json
        Console.WriteLine($"\nUpdating {remoteConfigPath}...");
        var versionInfo = new LauncherVersionInfo
        {
            Version = version,
            Filename = "Kaadu.exe",
            ReleaseNotes = $"Version {version} release"
        };

        var json = JsonSerializer.Serialize(versionInfo, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });

        // Ensure config directory exists
        CreateDirectoryRecursive(client, $"{RemoteBasePath}{ConfigPath}");

        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
        {
            client.UploadFile(stream, remoteConfigPath, true);
        }

        client.Disconnect();

        Console.WriteLine("\n=== Deployment Complete ===");
        Console.WriteLine($"Version: v{version}");
        Console.WriteLine($"Files:   {remoteLauncherDir}/");
        Console.WriteLine($"Config:  {remoteConfigPath}");
    }

    private static string FormatSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        var order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }

    private static void CreateDirectoryRecursive(SftpClient client, string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = "";

        foreach (var part in parts)
        {
            current += "/" + part;
            try
            {
                client.CreateDirectory(current);
            }
            catch
            {
                // Directory likely already exists
            }
        }
    }

    private static string? GetArgValue(string[] args, string key)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private static string ReadPassword()
    {
        var password = new System.Text.StringBuilder();
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Length--;
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);

        return password.ToString();
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: Kaadu.Deploy.exe <version> <launcher-exe-path> [options]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  <version>           Version number (e.g., 1.0.2)");
        Console.WriteLine("  <launcher-exe-path> Path to the compiled Kaadu.exe");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --host <host>       SFTP server hostname (default: test.swgtitan.org)");
        Console.WriteLine("  --user <user>       SFTP username");
        Console.WriteLine("  --password <pass>   SFTP password");
        Console.WriteLine();
        Console.WriteLine("Environment Variables:");
        Console.WriteLine("  SFTP_HOST           Alternative to --host");
        Console.WriteLine("  SFTP_USER           Alternative to --user");
        Console.WriteLine("  SFTP_PASSWORD       Alternative to --password");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  Kaadu.Deploy.exe 1.0.2 ./bin/Release/net9.0-windows/publish/Kaadu.exe");
    }
}

/// <summary>
/// Launcher version info for serialization.
/// </summary>
public sealed record LauncherVersionInfo
{
    public string Version { get; init; } = string.Empty;
    public string? ReleaseNotes { get; init; }
    public string? MinimumVersion { get; init; }
    public string Filename { get; init; } = "Kaadu.exe";
}
