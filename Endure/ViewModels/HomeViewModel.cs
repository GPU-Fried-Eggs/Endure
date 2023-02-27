using CommunityToolkit.Mvvm.ComponentModel;

namespace Endure.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private string searching;

    public HomeViewModel()
    {
        
    }

    partial void OnSearchingChanged(string value)
    {
        
    }
}