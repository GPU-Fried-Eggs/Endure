using Endure.ViewModels;

namespace Endure;

public partial class App
{
    public new static App Current => Application.Current as App ?? throw new ArgumentNullException();

    public Window? StartupWindow { get; private set; }

    public Action<AppTheme> ThemeChanged = _ => { };

    public AppTheme Theme
    {
        get =>
            Preferences.ContainsKey(nameof(Theme))
                ? Enum.Parse<AppTheme>(Preferences.Get(nameof(Theme), Enum.GetName(AppTheme.Light)) ?? string.Empty)
                : AppTheme.Light;
        set
        {
            Preferences.Set(nameof(Theme), value.ToString());
            Current.UserAppTheme = value;
            ThemeChanged(value);
        }
    }

    public Action<BackdropStyle> BackdropChanged = _ => { };

    public BackdropStyle BackdropStyle
    {
        get =>
            Preferences.ContainsKey(nameof(BackdropStyle))
                ? Enum.Parse<BackdropStyle>(Preferences.Get(nameof(BackdropStyle), Enum.GetName(BackdropStyle.Mica)) ?? string.Empty)
                : BackdropStyle.Mica;
        set
        {
            Preferences.Set(nameof(BackdropStyle), value.ToString());
            BackdropChanged(value);
        }
    }

    public App()
    {
        InitializeComponent();

        switch (Theme)
        {
            case AppTheme.Unspecified:
            case AppTheme.Light:
            default:
                Current.UserAppTheme = AppTheme.Light;
                break;
            case AppTheme.Dark:
                Current.UserAppTheme = AppTheme.Dark;
                break;
        }
        
        MainPage = Constants.Desktop ? new AppDesktopShell() : new AppMobileShell();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        StartupWindow = base.CreateWindow(activationState);

        if (Constants.Desktop)
        {
            StartupWindow.MinimumWidth = 768;
            StartupWindow.MinimumHeight = 432;

            StartupWindow.MaximumWidth = 1920;
            StartupWindow.MaximumHeight = 1080;

            StartupWindow.SizeChanged += OnResize;
        }

        return StartupWindow;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (StartupWindow is null) return;

        Shell.Current.FlyoutBehavior = StartupWindow.Width < 960 ? FlyoutBehavior.Flyout : FlyoutBehavior.Locked;
    }
}