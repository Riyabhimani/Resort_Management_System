using System;
using System.ComponentModel.DataAnnotations;

namespace Resort_Management_System_MVC.Models
{
    public class BookingModel
    {
        [Key]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Contact Number is required")]
        [Phone(ErrorMessage = "Invalid Contact Number")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "ID Proof is required")]
        [Display(Name = "ID Proof")]
        public string IdProof { get; set; } = null!;

        [Required(ErrorMessage = "Number of Persons is required")]
        [Range(1, 50, ErrorMessage = "Must be at least 1 person")]
        [Display(Name = "Number of Persons")]
        public int NumberOfPersons { get; set; }

        [Required(ErrorMessage = "Number of Rooms is required")]
        [Range(1, 20, ErrorMessage = "Must book at least 1 room")]
        [Display(Name = "Number of Rooms")]
        public int NumberOfRooms { get; set; }

        [Required(ErrorMessage = "Booking Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Room Type is required")]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; } = null!;

        [Required(ErrorMessage = "Advance Payment is required")]
        [Range(0, 100000, ErrorMessage = "Enter valid amount")]
        [Display(Name = "Advance Payment")]
        [DataType(DataType.Currency)]
        public string AdvancePayment { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Modified { get; set; }
    }
}
