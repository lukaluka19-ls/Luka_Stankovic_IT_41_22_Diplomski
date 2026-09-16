using FluentValidation;
using Igraonica.Application.DTOs;

namespace Igraonica.Application.Validators;

public class UpdatePackageRequestValidator : AbstractValidator<UpdatePackageRequest>
{
    public UpdatePackageRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PricePerChild).GreaterThanOrEqualTo(0);
    }
}

public class CreateTimeSlotRequestValidator : AbstractValidator<CreateTimeSlotRequest>
{
    public CreateTimeSlotRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime)
            .WithMessage("Vreme zavrsetka mora biti posle vremena pocetka.");
    }
}
