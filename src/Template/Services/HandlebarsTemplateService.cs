using HandlebarsDotNet;

namespace Avolutions.Baf.Core.Template.Services;

public class HandlebarsTemplateService : TemplateService<string, string>
{
    protected override Task<string> ApplyValuesToTemplateAsync(string template, IDictionary<string, string> values, CancellationToken ct)
    {
        var compiledTemplate = Handlebars.Compile(template);
        var result = compiledTemplate(values);
        return Task.FromResult(result);
    }
}