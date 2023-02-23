using Endure.ViewModels;

namespace Endure.Views;

public partial class ReviewPage
{
    public ReviewPage()
    {
        InitializeComponent();

        BindingContext = new ReviewViewModel();
    }
}