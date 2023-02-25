using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Endure.Models;
using Endure.Services;

namespace Endure.ViewModels;

public partial class ReviewViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<Memo> memos;

    private IMemoService m_memoService;

    public ReviewViewModel(IMemoService memoService)
    {
        m_memoService = memoService;
#if DEBUG
#endif
    }
}