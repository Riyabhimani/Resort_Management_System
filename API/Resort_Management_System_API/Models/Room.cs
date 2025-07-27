using System;
using System.Collections.Generic;

namespace Resort_Management_System_API.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal PricePerDay { get; set; }

    public string RoomStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
