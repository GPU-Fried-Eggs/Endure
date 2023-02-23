using Endure.ViewModels;

namespace Endure.Views;

public partial class SettingsPage
{
    public SettingsPage()
    {
        InitializeComponent();

        BindingContext = new SettingsViewModel();
    }
}