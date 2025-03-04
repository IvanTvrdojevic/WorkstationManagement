using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkstationManagement.Models;

public class UserWorkPosition
{
    public int Id { get; set; }
    public required string ProductName { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int WorkPositionId { get; set; }
    public WorkPosition? WorkPosition { get; set; }
}