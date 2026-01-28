namespace Kaadu.ViewModels;

/// <summary>
/// Service for navigating between views.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets the current view model.
    /// </summary>
    ViewModelBase? CurrentViewModel { get; }

    /// <summary>
    /// Occurs when navigation changes.
    /// </summary>
    event EventHandler<ViewModelBase>? Navigated;

    /// <summary>
    /// Navigates to the login view.
    /// </summary>
    void NavigateToLogin();

    /// <summary>
    /// Navigates to the launcher view.
    /// </summary>
    void NavigateToLauncher();

    /// <summary>
    /// Navigates to the admin view.
    /// </summary>
    void NavigateToAdmin();

    /// <summary>
    /// Navigates to the install path selection view.
    /// </summary>
    void NavigateToInstallPath();
}
