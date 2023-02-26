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

    private Window? m_startupWindow;

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
        m_startupWindow = base.CreateWindow(activationState);

        if (Constants.Desktop)
        {
            m_startupWindow.MinimumWidth = 768;
            m_startupWindow.MinimumHeight = 432;

            m_startupWindow.MaximumWidth = 1920;
            m_startupWindow.MaximumHeight = 1080;

            m_startupWindow.SizeChanged += OnResize;
        }

        return m_startupWindow;
    }

    private void OnSystemThemeChanged(object? sender, AppThemeChangedEventArgs args)
    {
        UserAppTheme = args.RequestedTheme;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (m_startupWindow is null) return;

        Shell.Current.FlyoutBehavior = m_startupWindow.Width < 960 ? FlyoutBehavior.Flyout : FlyoutBehavior.Locked;
    }
}