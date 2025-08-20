namespace Resort_Management_System_MVC.Models
{
    public class GuestServiceModel
    {
        public int GuestServiceId { get; set; }

        public int ReservationId { get; set; }

        public int ServiceId { get; set; }

        public int GuestId { get; set; }

        public int Quantity { get; set; }

        public DateTime DateRequested { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public string? FullName { get; set; }

        public string? ServiceName { get; set; }

    }
}
