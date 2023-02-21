namespace Endure;

public partial class App
{
    private Window? m_window;

    public App()
    {
        InitializeComponent();

        switch (Constants.Theme)
        {
            case AppTheme.Unspecified:
            case AppTheme.Light:
            default:
                if (Current != null) Current.UserAppTheme = AppTheme.Light;
                break;
            case AppTheme.Dark:
                if (Current != null) Current.UserAppTheme = AppTheme.Dark;
                break;
        }
        
        MainPage = Constants.Desktop ? new AppDesktopShell() : new AppMobileShell();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        m_window = base.CreateWindow(activationState);

        if (Constants.Desktop)
        {
            m_window.MinimumWidth = 960;
            m_window.MinimumHeight = 540;

            m_window.MaximumWidth = 1920;
            m_window.MaximumHeight = 1080;

            m_window.SizeChanged += OnResize;
        }

        return m_window;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (m_window is null) return;

        Shell.Current.FlyoutBehavior = m_window.Width < 1200 ? FlyoutBehavior.Flyout : FlyoutBehavior.Locked;
    }
}