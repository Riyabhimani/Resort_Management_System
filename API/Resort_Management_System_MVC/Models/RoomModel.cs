namespace Resort_Management_System_MVC.Models
{
    public class RoomModel
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
    }
}
