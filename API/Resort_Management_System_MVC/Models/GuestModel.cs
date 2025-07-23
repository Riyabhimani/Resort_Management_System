using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Resort_Management_System_MVC.Models
{
    public class GuestModel
    {

        [Key]
        public int GuestId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Contact number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15)]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "ID proof is required")]
        [Display(Name = "ID Proof Type")]
        [StringLength(50)]
        public string Idproof { get; set; } = null!;


        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Modified Date")]
        public DateTime? Modified { get; set; }

    }
}
