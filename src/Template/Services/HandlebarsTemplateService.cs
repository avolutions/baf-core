using HandlebarsDotNet;

namespace Avolutions.Baf.Core.Template.Services;

public class HandlebarsTemplateService : TemplateService<string, string>
{
    public override IReadOnlyList<string> ExtractFieldNames(Stream template)
    {
        throw new NotImplementedException();
    }

    public override Task<string> ApplyModelToTemplateAsync(string template, object model, CancellationToken ct = default)
    {
        var compiledTemplate = Handlebars.Compile(template);
        var result = compiledTemplate(model);
        return Task.FromResult(result);
    }

    public override Task<string> ApplyValuesToTemplateAsync(string template, IDictionary<string, string> values, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}