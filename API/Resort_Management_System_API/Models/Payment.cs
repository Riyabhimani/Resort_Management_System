using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Resort_Management_System_API.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int GuestId { get; set; }

    public int ReservationId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal AmountPaid { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual Guest Guest { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    public virtual Reservation Reservation { get; set; } = null!;
}
