using System.Collections.Specialized;
using System.Windows;
using Kaadu.ViewModels;

namespace Kaadu.Views;

/// <summary>
/// Popup window for admin console output.
/// </summary>
public partial class AdminConsoleWindow : Window
{
    public AdminConsoleWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is LauncherViewModel viewModel)
        {
            viewModel.ConsoleLog.CollectionChanged += OnConsoleLogChanged;
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (DataContext is LauncherViewModel viewModel)
        {
            viewModel.ConsoleLog.CollectionChanged -= OnConsoleLogChanged;
        }
    }

    private void OnConsoleLogChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Auto-scroll to the bottom when new items are added
        if (e.Action == NotifyCollectionChangedAction.Add && ConsoleListBox.Items.Count > 0)
        {
            ConsoleListBox.ScrollIntoView(ConsoleListBox.Items[^1]);
        }
    }
}
