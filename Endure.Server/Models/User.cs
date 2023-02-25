using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Endure.Server.Models;

[PrimaryKey(nameof(Id))]
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public List<Memo> Memos { get; set; }
}