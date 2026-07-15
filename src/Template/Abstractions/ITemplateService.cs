namespace Avolutions.Baf.Core.Template.Abstractions;

public interface ITemplateService
{
    IReadOnlyList<string> ExtractFieldNames(Stream template);
}

public interface ITemplateService<in TTemplate, TResult> : ITemplateService
{
    Task<TResult> ApplyModelToTemplateAsync(TTemplate template, object model, CancellationToken ct);
    Task<TResult> ApplyValuesToTemplateAsync(TTemplate template, IDictionary<string, string> values, CancellationToken ct);
}