using Endure.ViewModels;

namespace Endure.Views;

public partial class ReviewPage
{
    public ReviewPage(ReviewViewModel reviewViewModel)
    {
        InitializeComponent();

        BindingContext = reviewViewModel;
    }
}