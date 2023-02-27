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
        var source = new Memo[]
        {
            new() { MemoId = new Guid(), Name = "TestA", Summary = "SummaryA", Level = 1, Touch = DateTime.Now },
            new() { MemoId = new Guid(), Name = "TestB", Summary = "SummaryB", Level = 2, Touch = DateTime.Now },
            new() { MemoId = new Guid(), Name = "TestC", Summary = "SummaryC", Level = 3, Touch = DateTime.Now },
            new() { MemoId = new Guid(), Name = "TestD", Summary = "SummaryD", Level = 4, Touch = DateTime.Now }
        };
        Memos = new ObservableCollection<Memo>(source);
        //memos = new 
#endif
    }
}