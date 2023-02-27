using Endure.ViewModels;

namespace Endure.Views;

public partial class MainPage
{
    public MainPage(HomeViewModel homeViewModel)
    {
        InitializeComponent();

        BindingContext = homeViewModel;
    }
}