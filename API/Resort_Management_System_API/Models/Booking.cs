using System;
using System.Collections.Generic;

namespace Resort_Management_System_API.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Idproof { get; set; } = null!;

    public int NumberOfPersons { get; set; }

    public int NumberOfRoom { get; set; }

    public DateTime BookingDate { get; set; }

    public string RoomType { get; set; } = null!;

    public string AdvancePayment { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }
}
