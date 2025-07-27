using System.ComponentModel.DataAnnotations;

namespace Resort_Management_System_MVC.Models
{
    public class RoomModel
    {
        [Key]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room Number is required")]
        [StringLength(10, ErrorMessage = "Room Number cannot exceed 10 characters")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = null!;

        [Required(ErrorMessage = "Room Type is required")]
        [StringLength(50)]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Price per day is required")]
        [Range(0, 99999.99, ErrorMessage = "Enter a valid price")]
        [Display(Name = "Price Per Day")]
        public decimal PricePerDay { get; set; }

        [Required(ErrorMessage = "Room Status is required")]
        [StringLength(20)]
        [Display(Name = "Room Status")]
        public string RoomStatus { get; set; } = null!;

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        public DateTime? Modified { get; set; }
    }
}