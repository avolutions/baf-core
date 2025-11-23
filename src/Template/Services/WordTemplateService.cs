using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Avolutions.Baf.Core.Template.Services;

public class WordTemplateService : TemplateService<Stream, byte[]>
{
    protected override Task<byte[]> ApplyValuesToTemplateAsync(Stream template, IDictionary<string, string> values, CancellationToken ct)
    {
        // Copy to a writable, seekable stream
        var output = new MemoryStream();
        template.CopyTo(output);
        output.Position = 0;
        
        using (var document = WordprocessingDocument.Open(output, true))
        {
            var body = document.MainDocumentPart?.Document.Body;
            if (body != null)
            {
                ReplaceMergeFields(body, values);
            }
        }

        output.Position = 0;
        
        return Task.FromResult(output.ToArray());
    }
    
    private static void ReplaceMergeFields(OpenXmlElement root, IDictionary<string, string> values)
    {
        foreach (var fieldCode in root.Descendants<FieldCode>().ToList())
        {
            var instruction = fieldCode.Text;
            if (string.IsNullOrWhiteSpace(instruction))
            {
                continue;
            }

            if (!instruction.Contains("MERGEFIELD", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var fieldName = ExtractMergeFieldName(instruction);
            if (fieldName == null)
            {
                continue;
            }

            if (!values.TryGetValue(fieldName, out var replacement))
            {
                continue;
            }

            ReplaceComplexFieldResult(fieldCode, replacement);
        }
    }

    private static string? ExtractMergeFieldName(string instruction)
    {
        const string tag = "MERGEFIELD";
        var index = instruction.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return null;
        }

        var after = instruction.Substring(index + tag.Length).Trim();

        // Remove flags like \* MERGEFORMAT, \b, etc.
        var slashIndex = after.IndexOf('\\');
        if (slashIndex >= 0)
        {
            after = after[..slashIndex];
        }

        // Cut at first whitespace
        var spaceIndex = after.IndexOfAny([' ', '\r', '\n', '\t']);
        if (spaceIndex >= 0)
        {
            after = after[..spaceIndex];
        }

        // Handle MERGEFIELD Name AND MERGEFIELD "Name"
        after = after.Trim().Trim('"');

        return string.IsNullOrWhiteSpace(after) ? null : after;
    }
    
    private static void ReplaceComplexFieldResult(FieldCode fieldCode, string replacement)
    {
        var current = fieldCode.Parent;
        if (current == null)
        {
            return;
        }

        var resultTexts = new List<Text>();
        var inResult = false;

        while ((current = current.NextSibling()) != null)
        {
            var fieldChar = current.GetFirstChild<FieldChar>();
            if (fieldChar != null)
            {
                if (fieldChar.FieldCharType?.Value == FieldCharValues.Separate)
                {
                    inResult = true;
                    continue;
                }

                if (fieldChar.FieldCharType?.Value == FieldCharValues.End)
                {
                    break;
                }
            }

            if (inResult)
            {
                resultTexts.AddRange(current.Descendants<Text>());
            }
        }

        if (resultTexts.Count == 0)
        {
            return;
        }

        resultTexts[0].Text = replacement;

        for (var i = 1; i < resultTexts.Count; i++)
        {
            resultTexts[i].Text = string.Empty;
        }
    }
}