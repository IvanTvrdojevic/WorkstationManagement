using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkstationManagement.Models;

public class WorkPosition
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public ICollection<UserWorkPosition> UserWorkPositions { get; set; } = new List<UserWorkPosition>();
}