using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class PaymentValidator : AbstractValidator<Payment>
    {
        public PaymentValidator()
        {
            RuleFor(p => p.GuestId)
                .GreaterThan(0).WithMessage("Guest is required.");

            RuleFor(p => p.ReservationId)
                .GreaterThan(0).WithMessage("Reservation is required.");

            RuleFor(p => p.PaymentDate)
                .NotEmpty().WithMessage("Payment date is required.")
                .Must(date => date <= DateTime.Today).WithMessage("Payment date cannot be in the future.");

            RuleFor(p => p.AmountPaid)
                .GreaterThan(0).WithMessage("Amount paid must be greater than 0.");

            RuleFor(p => p.PaymentMethod)
                .NotNull().WithMessage("Payment method is required.")
                .Length(3, 20).WithMessage("Payment method must be between 3 and 20 characters.")
                .Matches("^[A-Za-z ]+$").WithMessage("Payment method must contain only letters and spaces.");

            RuleFor(p => p.PaymentStatus)
                .NotNull().WithMessage("Payment status is required.")
                .Must(status => status == "Completed" || status == "Pending" || status == "Failed")
                .WithMessage("Payment status must be either Paid, Pending, or Failed.");

            RuleFor(p => p.Created)
                .NotEmpty().WithMessage("Created date is required.");
        }
    }
}
