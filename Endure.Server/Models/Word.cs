using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Models;

[PrimaryKey(nameof(WordId))]
public class Word
{
    public int WordId { get; set; }
}