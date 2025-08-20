using System;
using System.ComponentModel.DataAnnotations;

namespace Resort_Management_System_MVC.Models
{
    public class PaymentModel
    {
        [Key]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Guest is required")]
        [Display(Name = "Guest")]
        public int GuestId { get; set; }

        [Required(ErrorMessage = "Reservation is required")]
        [Display(Name = "Reservation")]
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Payment date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Amount paid is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = null!;

        [Required(ErrorMessage = "Payment status is required")]
        [StringLength(50)]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = null!;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Modified Date")]
        public DateTime? Modified { get; set; }

        public string? FullName { get; set; }
    }
}
