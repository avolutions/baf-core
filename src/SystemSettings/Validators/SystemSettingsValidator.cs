using Avolutions.Baf.Core.SystemSettings.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Avolutions.Baf.Core.SystemSettings.Validators;

public class SystemSettingsValidator : AbstractValidator<SystemSettings>
{
    public SystemSettingsValidator(IStringLocalizer<SystemSettingsResources> localizer)
    {
        RuleFor(x => x.ApplicationTitle)
            .NotEmpty()
            .WithName(localizer["ApplicationTitle"]);
    }
}