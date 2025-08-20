namespace Resort_Management_System_API.Models;
using Microsoft.EntityFrameworkCore;


    [Keyless]
    public class Login
    {
        public string UserName { get; set; } = string.Empty;

        public string Role { get; set; }
       
        public string Password { get; set; } = string.Empty;
    }

