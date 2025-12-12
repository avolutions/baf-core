using PdfSharp;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;

namespace Avolutions.Baf.Core.Template.Services;

public class PdfTemplateService : TemplateService<Stream, byte[]>
{
    private static bool _fontsConfigured;

    public PdfTemplateService()
    {
        if (!_fontsConfigured && Capabilities.Build.IsCoreBuild)
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            _fontsConfigured = true;
        }
    }

    public override IReadOnlyList<string> ExtractFieldNames(Stream template)
    {
        throw new NotImplementedException();
    }

    public override Task<byte[]> ApplyValuesToTemplateAsync(Stream template, IDictionary<string, string> values, CancellationToken ct)
    {
        using var templateBuffer = new MemoryStream();
        template.CopyTo(templateBuffer);
        var templateBytes = templateBuffer.ToArray();

        using var inputStream = new MemoryStream(templateBytes);
        using var document = PdfReader.Open(inputStream, PdfDocumentOpenMode.Modify);

        var form = document.AcroForm;
        if (form.Fields.Count == 0)
        {
            using var outputNoForm = new MemoryStream();
            document.Save(outputNoForm);
            
            return Task.FromResult(outputNoForm.ToArray());
        }

        if (!form.Elements.ContainsKey("/NeedAppearances"))
        {
            form.Elements.Add("/NeedAppearances", new PdfBoolean(true));
        }
        else
        {
            form.Elements["/NeedAppearances"] = new PdfBoolean(true);
        }

        ApplyValuesToFields(form, values);

        using var output = new MemoryStream();
        document.Save(output);
        return Task.FromResult(output.ToArray());
    }
    
    private static void ApplyValuesToFields(PdfAcroForm form, IDictionary<string, string> values)
    {
        var fields = form.Fields;

        foreach (string fieldName in fields.DescendantNames)
        {
            if (!values.TryGetValue(fieldName, out var value))
            {
                var simpleName = GetSimpleName(fieldName);
                if (!values.TryGetValue(simpleName, out value))
                {
                    continue;
                }
            }

            var field = fields[fieldName];
            if (field is PdfTextField textField && !textField.ReadOnly)
            {
                textField.Value = new PdfString(value);
            }
        }
    }

    private static string GetSimpleName(string fieldName)
    {
        var lastDot = fieldName.LastIndexOf('.');
        if (lastDot >= 0 && lastDot < fieldName.Length - 1)
        {
            return fieldName[(lastDot + 1)..];
        }

        return fieldName;
    }
}