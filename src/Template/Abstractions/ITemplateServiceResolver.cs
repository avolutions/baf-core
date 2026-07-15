namespace Avolutions.Baf.Core.Template.Abstractions;

public interface ITemplateServiceResolver
{
    ITemplateService GetFieldExtractor(string extension);
    ITemplateService<TTemplate, TResult> GetTemplateService<TTemplate, TResult>(string extension);
}