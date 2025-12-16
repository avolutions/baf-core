namespace Avolutions.Baf.Core.Template.Abstractions;

public interface ITemplateService<in TTemplate, TResult>
{
    Task<TResult> ApplyModelToTemplateAsync(TTemplate template, object model, CancellationToken ct);
    IReadOnlyList<string> ExtractFieldNames(Stream template);
}