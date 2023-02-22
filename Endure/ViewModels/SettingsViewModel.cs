using CommunityToolkit.Mvvm.ComponentModel;

namespace Endure.ViewModels;

public enum BackdropStyle { Mica, Acrylic }

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private AppTheme theme;

    [ObservableProperty]
    private BackdropStyle style;

    [ObservableProperty]
    private string[] backdrops;

    public SettingsViewModel()
    {
        backdrops = Enum.GetNames(typeof(BackdropStyle));
        theme = App.Current.Theme;
        style = App.Current.BackdropStyle;
    }

    partial void OnStyleChanged(BackdropStyle value)
    {
        App.Current.BackdropStyle = value;
    }

    partial void OnThemeChanged(AppTheme value)
    {
        App.Current.Theme = value;
    }
}