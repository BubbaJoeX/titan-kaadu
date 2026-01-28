using CommunityToolkit.Mvvm.ComponentModel;

namespace Kaadu.ViewModels;

/// <summary>
/// Base class for all view models providing common functionality.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Clears the error message.
    /// </summary>
    protected void ClearError() => ErrorMessage = null;

    /// <summary>
    /// Sets an error message.
    /// </summary>
    protected void SetError(string message) => ErrorMessage = message;
}
