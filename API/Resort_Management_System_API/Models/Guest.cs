using System;
using System.Collections.Generic;

namespace Resort_Management_System_API.Models;

public partial class Guest
{
    public int GuestId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Idproof { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual ICollection<GuestService> GuestServices { get; set; } = new List<GuestService>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
