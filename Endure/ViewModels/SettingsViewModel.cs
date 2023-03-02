using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Endure.Services;

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

    private IPublicClientService m_publicClientService;

    public SettingsViewModel(IPublicClientService service)
    {
        m_publicClientService = service;

        sync = m_publicClientService.GetAccountFromCacheAsync().Result is null;

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

    [RelayCommand]
    public void ExpandCommand()
    {
        
    }

#if WINDOWS
    partial void OnStyleChanged(BackdropStyle value)
    {
        WinUI.App.Current.BackdropStyle = value;
    }
#endif
}