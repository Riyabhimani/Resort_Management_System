using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.UserName)
                .NotNull().WithMessage("Username is required.")
                .Length(3, 30).WithMessage("Username must be between 3 and 30 characters.")
                .Matches(@"^[A-Za-z0-9_]+$").WithMessage("Username can contain only letters, numbers, and underscores.");

            RuleFor(u => u.Password)
                .NotNull().WithMessage("Password is required.")
                .Length(6, 20).WithMessage("Password must be between 6 and 20 characters.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$")
                .WithMessage("Password must contain at least one letter and one number.");

            RuleFor(u => u.Role)
                .NotNull().WithMessage("Role is required.")
                .Must(role => role == "Admin" || role == "Receptionist" || role == "Manager" || role == "User" || role == "Guest" || role == "Visiter" || role == "Staff")
                .WithMessage("Role must be either Admin, Receptionist, or Manager.");

            RuleFor(u => u.Created)
                .NotEmpty().WithMessage("Created date is required.");

        }
    }
}
