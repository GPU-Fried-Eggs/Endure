using Endure.ViewModels;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Endure.WinUI;

public class AppBackdropStyleChangedEventArgs : EventArgs
{
    public BackdropStyle Style { get; }

    public AppBackdropStyleChangedEventArgs(BackdropStyle style) => Style = style;
}

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    public new static App Current => MauiWinUIApplication.Current as App ?? throw new ArgumentNullException();

    public event EventHandler<AppBackdropStyleChangedEventArgs>? BackdropChanged;

    public BackdropStyle BackdropStyle
    {
        get =>
            Preferences.ContainsKey(nameof(BackdropStyle))
                ? Enum.Parse<BackdropStyle>(Preferences.Get(nameof(BackdropStyle), Enum.GetName(BackdropStyle.Mica)) ?? string.Empty)
                : BackdropStyle.Mica;
        set
        {
            Preferences.Set(nameof(BackdropStyle), value.ToString());
            BackdropChanged?.Invoke(this, new AppBackdropStyleChangedEventArgs(value));
        }
    }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}