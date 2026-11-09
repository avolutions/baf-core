using System.Text.RegularExpressions;
using Avolutions.Baf.Core.Template.Attributes;
using PdfSharp;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;

namespace Avolutions.Baf.Core.Template.Services;

[TemplateExtension(".pdf")]
public partial class PdfTemplateService: TemplateService<Stream, byte[]>
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
        var fieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using var document = PdfReader.Open(template, PdfDocumentOpenMode.Modify);

        var form = document.AcroForm;
        if (form.Fields.Count == 0)
        {
            return [];
        }

        foreach (var (fieldName, field) in GetTerminalFields(form.Fields))
        {
            if (field is PdfTextField { ReadOnly: false })
            {
                fieldNames.Add(GetFieldName(fieldName));
            }
        }

        return fieldNames.ToList();
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
        foreach (var (fieldName, field) in GetTerminalFields(form.Fields))
        {
            if (!values.TryGetValue(GetFieldName(fieldName), out var value))
            {
                // Fallback for mappings created when only the last name segment was used
                var simpleName = GetSimpleName(fieldName);
                if (!values.TryGetValue(simpleName, out value))
                {
                    continue;
                }
            }

            if (field is PdfTextField textField && !textField.ReadOnly)
            {
                textField.Value = new PdfString(value);
            }
        }
    }

    /// <summary>
    /// Returns all terminal fields with their fully qualified names.
    /// A field copied in Acrobat (shown as "Name#0", "Name#1") is stored as one field whose kids are
    /// unnamed widget annotations. PdfSharp's DescendantNames skips such fields, so the tree is walked here.
    /// </summary>
    private static IEnumerable<(string Name, PdfAcroField Field)> GetTerminalFields(PdfAcroField.PdfAcroFieldCollection fields, string? parentName = null)
    {
        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var partialName = field.Elements.GetString(PdfAcroField.Keys.T);
            if (string.IsNullOrEmpty(partialName))
            {
                // Widget annotation, not a field
                continue;
            }

            var fullName = string.IsNullOrEmpty(parentName) ? partialName : $"{parentName}.{partialName}";

            if (field.HasKids && HasNamedKids(field.Fields))
            {
                foreach (var descendant in GetTerminalFields(field.Fields, fullName))
                {
                    yield return descendant;
                }
            }
            else
            {
                yield return (fullName, field);
            }
        }
    }

    private static bool HasNamedKids(PdfAcroField.PdfAcroFieldCollection fields)
    {
        for (var i = 0; i < fields.Count; i++)
        {
            if (!string.IsNullOrEmpty(fields[i].Elements.GetString(PdfAcroField.Keys.T)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the fully qualified name. A dot in a field name (e.g. "gesetzl. Mwst") creates a
    /// hierarchy in the PDF, so only using the last segment would lose part of the name.
    /// </summary>
    private static string GetFieldName(string fieldName)
    {
        // Some tools store copies as separate fields named "Name#0", "Name#1", ...
        return CopySuffixRegex().Replace(fieldName, string.Empty);
    }

    private static string GetSimpleName(string fieldName)
    {
        var lastDot = fieldName.LastIndexOf('.');
        if (lastDot >= 0 && lastDot < fieldName.Length - 1)
        {
            fieldName = fieldName[(lastDot + 1)..];
        }

        return GetFieldName(fieldName);
    }

    [GeneratedRegex(@"#\d+$")]
    private static partial Regex CopySuffixRegex();
}