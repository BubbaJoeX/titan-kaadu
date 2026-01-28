using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Kaadu.Models;

namespace Kaadu.Services;

/// <summary>
/// Implementation of authentication service using HTTPS endpoints.
/// </summary>
public sealed class AuthService : IAuthService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IAdminService _adminService;
    private bool _disposed;

    public AuthService(IAdminService adminService)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        _adminService = adminService;
    }

    public async Task<AuthResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(password);

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new AuthResult(false, "Username and password are required.");
        }

        try
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["user_name"] = username,
                ["user_password"] = password,
                ["secretKey"] = "330511"
            });

            var response = await _httpClient.PostAsync(AppConfiguration.AuthUrl, content, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return new AuthResult(false, $"Authentication failed: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            // Parse response - adjust based on actual API response format
            if (responseContent.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                responseContent.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
                responseContent.Contains("failed", StringComparison.OrdinalIgnoreCase))
            {
                return new AuthResult(false, "Invalid username or password.");
            }

            // Get admin level
            var adminLevel = await _adminService.GetAdminLevelAsync(username, cancellationToken).ConfigureAwait(false);

            var session = new UserSession(
                username, 
                true, 
                adminLevel, 
                null, // SessionToken
                password, // Store password for auto-login
                AppConfiguration.GameLoginServerAddress); // Server address
            return new AuthResult(true, null, session);
        }
        catch (HttpRequestException ex)
        {
            return new AuthResult(false, $"Network error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return new AuthResult(false, "Authentication request timed out.");
        }
        catch (Exception ex)
        {
            return new AuthResult(false, $"Authentication error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}
