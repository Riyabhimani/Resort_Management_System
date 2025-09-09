using System;
using System.ComponentModel.DataAnnotations;

namespace Resort_Management_System_MVC.Models
{
    public class GuestServiceModel
    {
        [Key]
        public int GuestServiceId { get; set; }

        [Required(ErrorMessage = "Reservation is required")]
        [Display(Name = "Reservation")]
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Reservation Status is required")]
        [StringLength(50)]
        [Display(Name = "Reservation Status")]
        public string ReservationStatus { get; set; }

        [Required(ErrorMessage = "Service is required")]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Guest is required")]
        [Display(Name = "Guest")]
        public int GuestId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Date Requested is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date Requested")]
        public DateTime DateRequested { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Modified { get; set; }

        [StringLength(100)]
        [Display(Name = "Guest Name")]
        public string? FullName { get; set; }

        [StringLength(100)]
        [Display(Name = "Service Name")]
        public string? ServiceName { get; set; }
    }
}
