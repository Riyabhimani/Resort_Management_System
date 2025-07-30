using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(r => r.GuestId)
                .GreaterThan(0).WithMessage("Guest is required.");

            RuleFor(r => r.RoomId)
                .GreaterThan(0).WithMessage("Room is required.");

            RuleFor(r => r.CheckInDate)
                .NotEmpty().WithMessage("Check-in date is required.")
                .Must(date => date >= DateTime.Today).WithMessage("Check-in date cannot be in the past.");

            RuleFor(r => r.CheckOutDate)
                .NotEmpty().WithMessage("Check-out date is required.")
                .GreaterThan(r => r.CheckInDate).WithMessage("Check-out date must be after check-in date.");

            RuleFor(r => r.BookingDate)
                .NotEmpty().WithMessage("Booking date is required.")
                .Must(date => date <= DateTime.Today).WithMessage("Booking date cannot be in the future.");

            RuleFor(r => r.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than 0.");

            RuleFor(r => r.ReservationStatus)
                .NotNull().WithMessage("Reservation status is required.")
                .Must(status => status == "Pending" || status == "Confirmed" || status == "Cancelled")
                .WithMessage("Reservation status must be either Pending, Confirmed, or Cancelled.");

            RuleFor(r => r.Created)
                .NotEmpty().WithMessage("Created date is required.");
        }
    }
}
