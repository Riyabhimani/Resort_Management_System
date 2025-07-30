using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator()
        {
            RuleFor(r => r.RoomNumber)
                .NotNull().WithMessage("Room number is required.")
                .Length(1, 10).WithMessage("Room number must be between 1 and 10 characters.")
                .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("Room number can contain only letters, numbers, and hyphens.");

            RuleFor(r => r.RoomType)
                .NotNull().WithMessage("Room type is required.")
                .Length(3, 30).WithMessage("Room type must be between 3 and 30 characters.")
                .Matches(@"^[A-Za-z ]+$").WithMessage("Room type can contain only letters and spaces.");

            RuleFor(r => r.Description)
                .NotNull().WithMessage("Description is required.")
                .Length(5, 200).WithMessage("Description must be between 5 and 200 characters.");

            RuleFor(r => r.PricePerDay)
                .GreaterThan(0).WithMessage("Price per day must be greater than 0.");

            RuleFor(r => r.RoomStatus)
                .NotNull().WithMessage("Room status is required.")
                .Must(status => status == "Available" || status == "Occupied" || status == "Maintenance")
                .WithMessage("Room status must be either Available, Occupied, or Maintenance.");

        }
    }
}
