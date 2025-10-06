﻿using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Template.Abstractions;
using Avolutions.Baf.Core.Template.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Template;

public class TemplateModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ITemplateService, TemplateService>();
    }
}