using CommunityToolkit.Mvvm.ComponentModel;
using Endure.Models;
using Endure.Resources.Strings;

namespace Endure.ViewModels;

public class ShellViewModel : ObservableObject
{
    public AppSection Main { get; }

    public AppSection Settings { get; }

    public ShellViewModel()
    {
        Main = new AppSection
        {
            Title = AppResource.Home,
            Icon = "ic_fluent_home.png"
        };
        Settings = new AppSection
        {
            Title = AppResource.Settings,
            Icon = "ic_fluent_settings.png"
        };
    }
}