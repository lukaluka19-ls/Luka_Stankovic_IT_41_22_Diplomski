using FluentValidation;
using Igraonica.Application.DTOs;
using Igraonica.Application.Options;
using Microsoft.Extensions.Options;

namespace Igraonica.Application.Validators;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator(IOptions<BusinessRulesOptions> rules)
    {
        var maxAdvanceDays = rules.Value.MaxAdvanceBookingDays;

        RuleFor(x => x.ChildName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ChildAge).InclusiveBetween(1, 14)
            .WithMessage("Uzrast deteta mora biti izmedju 1 i 14 godina.");
        RuleFor(x => x.NumberOfChildren).InclusiveBetween(1, 40)
            .WithMessage("Broj dece mora biti izmedju 1 i 40.");
        RuleFor(x => x.EventDate)
            .GreaterThan(x => DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Datum proslave mora biti u buducnosti.")
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(maxAdvanceDays))
            .WithMessage($"Rezervacija se moze napraviti najvise {maxAdvanceDays} dana unapred.");
        RuleFor(x => x.TimeSlotId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
    }
}

public class CancelReservationRequestValidator : AbstractValidator<CancelReservationRequest>
{
    public CancelReservationRequestValidator(IOptions<BusinessRulesOptions> rules)
    {
        RuleFor(x => x.Reason).NotEmpty()
            .MinimumLength(rules.Value.MinCancellationReasonLength)
            .WithMessage($"Razlog otkazivanja mora imati najmanje " +
                         $"{rules.Value.MinCancellationReasonLength} znakova.");
    }
}
