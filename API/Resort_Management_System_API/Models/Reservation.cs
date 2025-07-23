using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Resort_Management_System_API.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int GuestId { get; set; }

    public int RoomId { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public DateTime BookingDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string ReservationStatus { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [JsonIgnore]
    public virtual Guest? Guest { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<GuestService>? GuestServices { get; set; } = new List<GuestService>();

    [JsonIgnore]
    public virtual ICollection<Payment>? Payments { get; set; } = new List<Payment>();

    [JsonIgnore]
    public virtual Room? Room { get; set; } = null!;
}
