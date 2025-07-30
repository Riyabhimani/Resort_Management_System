using System.Diagnostics.Metrics;
using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
   
        public class GuestValidator : AbstractValidator<Guest>
        {
            public GuestValidator()
            {
            RuleFor(g => g.FullName)
                    .NotNull().WithMessage("Full name must not be empty.")
                    .Length(3, 50).WithMessage("Full name must be between 3 and 50 characters.")
                    .Matches("^[A-Za-z ]*$").WithMessage("Full name must contain only letters and spaces.");

            RuleFor(g => g.Email)
                    .NotNull().WithMessage("Email is required.")
                    .NotEmpty().WithMessage("Email must not be empty.")
                    .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(g => g.ContactNumber)
                    .NotNull().WithMessage("Contact number is required.")
                    .Matches(@"^[0-9]{10}$").WithMessage("Contact number must be exactly 10 digits.");

            RuleFor(g => g.Address)
                    .NotNull().WithMessage("Address is required.")
                    .Length(5, 100).WithMessage("Address must be between 5 and 100 characters.");

            RuleFor(g => g.Idproof)
                .NotNull().WithMessage("ID proof is required.")
                .Length(4, 30).WithMessage("ID proof must be between 4 and 30 characters.")
                .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("ID proof must contain only letters, numbers, or hyphens.");


        }
    }

    }

