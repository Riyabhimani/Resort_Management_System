using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Resort_Management_System_API.Models;

public partial class GuestService
{
    public int GuestServiceId { get; set; }

    public int ReservationId { get; set; }

    public int ServiceId { get; set; }

    public int Quantity { get; set; }

    public DateTime DateRequested { get; set; }

    public int GuestId { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual Guest Guest { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    public virtual Reservation Reservation { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    public virtual Service Service { get; set; } = null!;
}
