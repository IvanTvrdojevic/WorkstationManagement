using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkstationManagement.Models;

public class Role
{
    public int Id { get; set; }
    public required string RoleName { get; set; }
    public required string Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}