using FluentValidation;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_MVC.ValidationClass
{
    public class ServiceValidator : AbstractValidator<Service>
    {
        public ServiceValidator()
        {
            RuleFor(s => s.ServiceName)
                .NotNull().WithMessage("Service name is required.")
                .Length(3, 50).WithMessage("Service name must be between 3 and 50 characters.")
                .Matches(@"^[A-Za-z ]+$").WithMessage("Service name can contain only letters and spaces.");

            RuleFor(s => s.Description)
                .NotNull().WithMessage("Description is required.")
                .Length(5, 200).WithMessage("Description must be between 5 and 200 characters.");

            RuleFor(s => s.ServiceCost)
                .GreaterThan(0).WithMessage("Service cost must be greater than 0.")
                .Must(value => value == Math.Round(value, 2)).WithMessage("Service cost must have up to 2 decimal places.");

            RuleFor(s => s.ServiceStartTime)
                .NotNull().WithMessage("Service start time is required.");

            RuleFor(s => s.ServiceEndTime)
                .NotNull().WithMessage("Service end time is required.")
                .Must((s, endTime) => endTime > s.ServiceStartTime)
                .WithMessage("Service end time must be after start time.");

        }
    }
}
