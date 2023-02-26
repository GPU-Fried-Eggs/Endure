using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Models;

[PrimaryKey(nameof(MemoId))]
public class Memo
{
    /// <summary>
    /// Unique id (ms style :D) of the memory
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid MemoId { get; set; }
    
    /// <summary>
    /// Name of the memory, e.g. human / ihmisen / 人类 / 人々
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Summary of memory
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Current level of memory stage.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// The last touch of this memory
    /// </summary>
    public DateTime Touch { get; set; }

    /// <summary>
    /// The word model.
    /// </summary>
    public Word Word { get; set; }
}