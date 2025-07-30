using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class StaffValidator : AbstractValidator<Staff>
    {
        public StaffValidator()
        {
            RuleFor(s => s.FullName)
                .NotNull().WithMessage("Full name is required.")
                .Length(3, 50).WithMessage("Full name must be between 3 and 50 characters.")
                .Matches(@"^[A-Za-z ]+$").WithMessage("Full name can contain only letters and spaces.");

            RuleFor(s => s.Role)
                .NotNull().WithMessage("Role is required.")
                .Length(3, 30).WithMessage("Role must be between 3 and 30 characters.");

            RuleFor(s => s.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email must not be empty.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(s => s.ContactNumber)
                .NotNull().WithMessage("Contact number is required.")
                .Matches(@"^[0-9]{10}$").WithMessage("Contact number must be exactly 10 digits.");

            RuleFor(s => s.JoiningDate)
                .NotEmpty().WithMessage("Joining date is required.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Joining date cannot be in the future.");

            RuleFor(s => s.Salary)
                .NotNull().WithMessage("Salary is required.")
                .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Salary must be a valid amount with up to 2 decimal places.");

        }
    }
}
