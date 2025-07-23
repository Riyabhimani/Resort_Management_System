namespace Resort_Management_System_MVC.Models
{
    public class PaymentModel
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

    }
}
