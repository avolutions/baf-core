using Avolutions.Baf.Core.Lookups.Models;
using Avolutions.Baf.Core.Lookups.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Avolutions.Baf.Core.Lookups.Validators;

public abstract class LookupTranslationValidator<T> : AbstractValidator<T>
    where T : LookupTranslation
{
    protected LookupTranslationValidator(IStringLocalizer<LookupResources> localizer)
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithName(localizer["Field.Value"]);
    }
}