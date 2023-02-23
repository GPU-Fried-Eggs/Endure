using CommunityToolkit.Mvvm.ComponentModel;
using Endure.Models;
using Endure.Resources.Strings;

namespace Endure.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime currentTime;

    public AppSection Main { get; }

    public AppSection Review { get; }

    public AppSection Settings { get; }

    public ShellViewModel()
    {
        if (Constants.Desktop)
        {
            var timer = App.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (_, _) => CurrentTime = DateTime.Now;
            timer.Start();
        }

        currentTime = DateTime.Now;

        Main = new AppSection { Title = AppResource.Home };
        Review = new AppSection { Title = AppResource.Review };
        Settings = new AppSection { Title = AppResource.Settings };
    }
}