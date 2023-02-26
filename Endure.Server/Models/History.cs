using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Models;

[PrimaryKey(nameof(HistoryId))]
public class History
{
    public int HistoryId { get; set; }
    
    public Memo Data { get; set; }

    public DateTime VisitTime { get; set; }
}