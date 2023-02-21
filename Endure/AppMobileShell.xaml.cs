using Endure.ViewModels;

namespace Endure;

public partial class AppMobileShell
{
    public AppMobileShell()
    {
        InitializeComponent();

        BindingContext = new ShellViewModel();
    }
}