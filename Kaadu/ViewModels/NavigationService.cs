namespace Kaadu.ViewModels;

/// <summary>
/// Service implementation for view navigation.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly Func<LoginViewModel> _loginViewModelFactory;
    private readonly Func<LauncherViewModel> _launcherViewModelFactory;
    private readonly Func<AdminViewModel> _adminViewModelFactory;
    private readonly Func<InstallPathViewModel> _installPathViewModelFactory;

    private ViewModelBase? _currentViewModel;

    public NavigationService(
        Func<LoginViewModel> loginViewModelFactory,
        Func<LauncherViewModel> launcherViewModelFactory,
        Func<AdminViewModel> adminViewModelFactory,
        Func<InstallPathViewModel> installPathViewModelFactory)
    {
        _loginViewModelFactory = loginViewModelFactory;
        _launcherViewModelFactory = launcherViewModelFactory;
        _adminViewModelFactory = adminViewModelFactory;
        _installPathViewModelFactory = installPathViewModelFactory;
    }

    public ViewModelBase? CurrentViewModel => _currentViewModel;

    public event EventHandler<ViewModelBase>? Navigated;

    public void NavigateToLogin()
    {
        _currentViewModel = _loginViewModelFactory();
        Navigated?.Invoke(this, _currentViewModel);
    }

    public void NavigateToLauncher()
    {
        _currentViewModel = _launcherViewModelFactory();
        Navigated?.Invoke(this, _currentViewModel);
    }

    public void NavigateToAdmin()
    {
        _currentViewModel = _adminViewModelFactory();
        Navigated?.Invoke(this, _currentViewModel);
    }

    public void NavigateToInstallPath()
    {
        _currentViewModel = _installPathViewModelFactory();
        Navigated?.Invoke(this, _currentViewModel);
    }
}
