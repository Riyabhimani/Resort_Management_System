using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Resort_Management_System_MVC.Models
{
    public class ReservationModel
    {
        [Key]
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "Guest is required.")]
        [Display(Name = "Guest")]
        public int GuestId { get; set; }

        [Required(ErrorMessage = "Room is required.")]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Check-in date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative.")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Reservation status is required.")]
        [StringLength(50)]
        [Display(Name = "Reservation Status")]
        public string ReservationStatus { get; set; } = null!;

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? Modified { get; set; }
    }
}
