using System.ComponentModel.DataAnnotations;

namespace Resort_Management_System_MVC.Models
{
    public class ServiceModel
    {
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters")]
        public string ServiceName { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Service cost is required")]
        [Range(1, 100000, ErrorMessage = "Cost must be greater than 0")]
        public decimal ServiceCost { get; set; }

        [Required(ErrorMessage = "Service start time is required")]
        public TimeOnly ServiceStartTime { get; set; }

        [Required(ErrorMessage = "Service end time is required")]
        public TimeOnly ServiceEndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? Modified { get; set; }
    }
}
