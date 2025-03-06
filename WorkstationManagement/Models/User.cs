using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkstationManagement.Models;

public class User{
    public int Id { get; set; }
    public required string  FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required bool ChangePwNeeded { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; } 
    public ICollection<UserWorkPosition> UserWorkPositions { get; set; } = new List<UserWorkPosition>();
}