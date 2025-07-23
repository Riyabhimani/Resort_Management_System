using System;
using System.Collections.Generic;

namespace Resort_Management_System_API.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string FullName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public DateTime JoiningDate { get; set; }

    public string Salary { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }
}
