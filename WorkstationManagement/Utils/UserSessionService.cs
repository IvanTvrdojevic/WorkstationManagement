using System.Collections.Generic;
using WorkstationManagement.Models;

namespace WorkstationManagement.Utils;

public class UserSessionService : User 
{
    public User? CurrentUser { get; set; }
}