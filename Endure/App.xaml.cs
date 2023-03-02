namespace Endure;

public partial class App
{
    public new static App Current => Application.Current as App ?? throw new ArgumentNullException();

    public bool DataSync { get; set; }

    public AppTheme Theme
    {
        get =>
            Preferences.ContainsKey(nameof(Theme))
                ? Enum.Parse<AppTheme>(Preferences.Get(nameof(Theme), Enum.GetName(AppTheme.Light)) ?? string.Empty)
                : Current.RequestedTheme;
        set
        {
            Preferences.Set(nameof(Theme), value.ToString());
            switch (value)
            {
                case AppTheme.Unspecified:
                    Current.UserAppTheme = Current.PlatformAppTheme;
                    RequestedThemeChanged += OnSystemThemeChanged;
                    break;
                default:
                    Current.UserAppTheme = value;
                    RequestedThemeChanged -= OnSystemThemeChanged;
                    break;
            }
        }
    }

    public Window? StartupWindow { get; private set; }

    public App()
    {
        InitializeComponent();

        switch (Theme)
        {
            case AppTheme.Unspecified:
                RequestedThemeChanged += OnSystemThemeChanged;
                break;
            case AppTheme.Light:
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

    private void OnSystemThemeChanged(object? sender, AppThemeChangedEventArgs args)
    {
        UserAppTheme = args.RequestedTheme;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (StartupWindow is null) return;

        Shell.Current.FlyoutBehavior = StartupWindow.Width < 960 ? FlyoutBehavior.Flyout : FlyoutBehavior.Locked;
    }
}