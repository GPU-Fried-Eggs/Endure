using CommunityToolkit.Mvvm.ComponentModel;

namespace Endure.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isDarkModeEnabled;

    public SettingsViewModel()
    {
        isDarkModeEnabled = Constants.Theme == AppTheme.Dark;
    }

    partial void OnIsDarkModeEnabledChanged(bool value)
    {
        Constants.Theme = value ? AppTheme.Dark : AppTheme.Light;
        if (Application.Current != null)
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
    }
}