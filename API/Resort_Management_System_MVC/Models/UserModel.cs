using System.ComponentModel.DataAnnotations;

namespace Resort_Management_System_MVC.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [StringLength(100, ErrorMessage = "User name cannot exceed 100 characters")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;


        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;


        [Required(ErrorMessage = "Role is required")]
        [StringLength(20)]
        public string Role { get; set; } = null!;


        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; } 


        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        public DateTime? Modified { get; set; }


        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
