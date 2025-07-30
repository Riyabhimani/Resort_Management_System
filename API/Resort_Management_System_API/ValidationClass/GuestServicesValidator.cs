using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class GuestServicesValidator : AbstractValidator<GuestService>
    {
        public GuestServicesValidator()
        {
            RuleFor(gs => gs.ReservationId)
                .GreaterThan(0).WithMessage("Reservation is required.");

            RuleFor(gs => gs.ServiceId)
                .GreaterThan(0).WithMessage("Service is required.");

            RuleFor(gs => gs.GuestId)
                .GreaterThan(0).WithMessage("Guest is required.");

            RuleFor(gs => gs.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(gs => gs.DateRequested)
                .NotEmpty().WithMessage("Date requested is required.")
                .Must(date => date <= DateTime.Today).WithMessage("Date requested cannot be in the future.");

            RuleFor(gs => gs.Created)
                .NotEmpty().WithMessage("Created date is required.");
        }
    }
}
