using Endure.ViewModels;

namespace Endure;

public partial class AppDesktopShell
{
    public AppDesktopShell()
    {
        InitializeComponent();

        BindingContext = new ShellViewModel();
    }
}