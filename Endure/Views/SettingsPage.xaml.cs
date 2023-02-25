using Endure.ViewModels;

namespace Endure.Views;

public partial class SettingsPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();

        BindingContext = settingsViewModel;
    }
}