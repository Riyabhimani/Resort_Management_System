using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Resort_Management_System_API.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal ServiceCost { get; set; }

    public TimeOnly ServiceStartTime { get; set; }

    public TimeOnly ServiceEndTime { get; set; }

    public bool IsActive { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual ICollection<GuestService> GuestServices { get; set; } = new List<GuestService>();
}
