using CommunityToolkit.Mvvm.ComponentModel;

namespace Endure.ViewModels;

public enum BackdropStyle { None, Mica, Acrylic }

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private AppTheme theme;

    [ObservableProperty]
    private bool sync;

    [ObservableProperty]
    private string[] backdrops;

    [ObservableProperty]
    private BackdropStyle style;

    public SettingsViewModel()
    {
        theme = App.Current.Theme;
#if WINDOWS
        backdrops = Enum.GetNames(typeof(BackdropStyle));
        style = WinUI.App.Current.BackdropStyle;
#else
        backdrops = new[] { "WinSDK" };
        style = default;
#endif
    }

    partial void OnThemeChanged(AppTheme value)
    {
        App.Current.Theme = value;
    }

    partial void OnSyncChanged(bool value)
    {
        
    }

#if WINDOWS
    partial void OnStyleChanged(BackdropStyle value)
    {
        WinUI.App.Current.BackdropStyle = value;
    }
#endif
}