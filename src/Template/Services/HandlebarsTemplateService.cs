using HandlebarsDotNet;

namespace Avolutions.Baf.Core.Template.Services;

public class HandlebarsTemplateService : TemplateService<string, string>
{
    public override IReadOnlyList<string> ExtractFieldNames(Stream template)
    {
        throw new NotImplementedException();
    }

    public override Task<string> ApplyValuesToTemplateAsync(string template, IDictionary<string, string> values, CancellationToken ct)
    {
        var compiledTemplate = Handlebars.Compile(template);
        var result = compiledTemplate(values);
        return Task.FromResult(result);
    }
}