using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Models;

[PrimaryKey(nameof(Id))]
public class Memo
{
    /// <summary>
    /// Unique id (ms style :D) of the memory
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
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
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Touch { get; set; }
    
    public Word Word { get; set; }
}