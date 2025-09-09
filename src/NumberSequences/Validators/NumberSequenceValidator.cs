using Avolutions.Baf.Core.NumberSequences.Models;
using Avolutions.Baf.Core.NumberSequences.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Avolutions.Baf.Core.NumberSequences.Validators;

public class NumberSequenceValidator : AbstractValidator<NumberSequence>
{
    public NumberSequenceValidator(IStringLocalizer<NumberSequenceResources> localizer)
    {
        RuleFor(x => x.NextNumber)
            .GreaterThan(0)
            .WithMessage(localizer["Validation.NextNumber"]);

        RuleFor(x => x.NumberLength)
            .InclusiveBetween(1, 6)
            .WithMessage(localizer["Validation.NumberLength"]);

        RuleFor(x => x.Prefix)
            .MaximumLength(2)
            .WithMessage(localizer["Validation.Prefix"]);
    }
}