using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class BookingValidator : AbstractValidator<Booking>
    {
        public BookingValidator()
        {
            RuleFor(b => b.FullName)
                .NotNull().WithMessage("Full name is required.")
                .Length(3, 50).WithMessage("Full name must be between 3 and 50 characters.")
                .Matches("^[A-Za-z ]*$").WithMessage("Full name must contain only letters and spaces.");

            RuleFor(b => b.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email must not be empty.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(b => b.ContactNumber)
                .NotNull().WithMessage("Contact number is required.")
                .Matches(@"^[0-9]{10}$").WithMessage("Contact number must be exactly 10 digits.");

            RuleFor(b => b.Address)
                .NotNull().WithMessage("Address is required.")
                .Length(5, 100).WithMessage("Address must be between 5 and 100 characters.");

            RuleFor(b => b.Idproof)
                .NotNull().WithMessage("ID proof is required.")
                .Length(4, 30).WithMessage("ID proof must be between 4 and 30 characters.")
                .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("ID proof must contain only letters, numbers, or hyphens.");

            RuleFor(b => b.NumberOfPersons)
                .GreaterThan(0).WithMessage("Number of persons must be greater than 0.");

            RuleFor(b => b.NumberOfRoom)
                .GreaterThan(0).WithMessage("Number of rooms must be greater than 0.");

            RuleFor(b => b.BookingDate)
                .NotEmpty().WithMessage("Booking date is required.")
                .Must(date => date >= DateTime.Today).WithMessage("Booking date cannot be in the past.");

            RuleFor(b => b.RoomType)
                .NotNull().WithMessage("Room type is required.")
                .Length(3, 30).WithMessage("Room type must be between 3 and 30 characters.");

            RuleFor(b => b.AdvancePayment)
                .NotNull().WithMessage("Advance payment is required.")
                .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Advance payment must be a valid number.");

            RuleFor(b => b.Created)
                .NotEmpty().WithMessage("Created date is required.");
        }
    }
}
