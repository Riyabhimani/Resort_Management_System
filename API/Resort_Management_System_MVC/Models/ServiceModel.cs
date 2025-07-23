
namespace Resort_Management_System_MVC.Models
{
    public class ServiceModel
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

    }
}
