using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kaadu.Models;
using Kaadu.Services;
using Microsoft.Win32;

namespace Kaadu.ViewModels;

/// <summary>
/// ViewModel for the install path selection screen.
/// </summary>
public sealed partial class InstallPathViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly INavigationService _navigationService;
    private readonly Action _onPathConfigured;

    [ObservableProperty]
    private string _installPath = AppConfiguration.DefaultInstallPath;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isStatusError;

    public InstallPathViewModel(
        ISettingsService settingsService,
        INavigationService navigationService,
        Action onPathConfigured)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _onPathConfigured = onPathConfigured ?? throw new ArgumentNullException(nameof(onPathConfigured));

        // Start with the current setting or default
        InstallPath = _settingsService.InstallPath;
    }

    [RelayCommand]
    private void Browse()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Installation Folder",
            InitialDirectory = Directory.Exists(InstallPath) 
                ? InstallPath 
                : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        };

        if (dialog.ShowDialog() == true)
        {
            InstallPath = dialog.FolderName;
        }
    }

    [RelayCommand]
    private async Task UseDefaultAsync()
    {
        InstallPath = AppConfiguration.DefaultInstallPath;
        await SaveAndContinueAsync();
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (string.IsNullOrWhiteSpace(InstallPath))
        {
            SetStatus("Please select an installation path.", isError: true);
            return;
        }

        await SaveAndContinueAsync();
    }

    private async Task SaveAndContinueAsync()
    {
        ClearError();
        SetStatus("Saving settings...", isError: false);
        IsBusy = true;

        try
        {
            // Validate the path
            if (!IsValidPath(InstallPath))
            {
                SetStatus("Invalid path. Please select a valid folder.", isError: true);
                return;
            }

            // Try to create the directory if it doesn't exist
            try
            {
                Directory.CreateDirectory(InstallPath);
            }
            catch (Exception ex)
            {
                SetStatus($"Cannot create folder: {ex.Message}", isError: true);
                return;
            }

            // Save the settings
            await _settingsService.SetInstallPathAsync(InstallPath);

            // Update the global configuration
            AppConfiguration.InstallPath = InstallPath;

            SetStatus("Settings saved!", isError: false);

            // Notify that path is configured and navigate
            _onPathConfigured();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _navigationService.NavigateToLauncher();
            });
        }
        catch (Exception ex)
        {
            SetStatus($"Error saving settings: {ex.Message}", isError: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        try
        {
            // Check if it's a valid path format
            _ = Path.GetFullPath(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void SetStatus(string message, bool isError)
    {
        StatusMessage = message;
        IsStatusError = isError;
        if (isError)
        {
            SetError(message);
        }
    }
}
