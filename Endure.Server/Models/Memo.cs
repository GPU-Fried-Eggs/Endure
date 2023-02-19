using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Endure.Server.Models;

public class Memo
{
    /// <summary>
    /// Unique id (ms style :D) of the memory
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of the memory, e.g. human / ihmisen / 人类 / 人々
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Summary of memory TODO: link to other tables
    /// </summary>
    [Required]
    public string Summary { get; set; }
    
    /// <summary>
    /// The last touch of this memory
    /// </summary>
    public DateTime Touch { get; set; }
}