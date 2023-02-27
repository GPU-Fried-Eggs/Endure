namespace Endure.Models;

public class Memo
{
    public Guid? MemoId { get; set; }

    public string? Name { get; set; }

    public string? Summary { get; set; }

    public int Level { get; set; }

    public DateTime Touch { get; set; }
}