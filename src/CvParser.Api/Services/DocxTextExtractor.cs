using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CvParser.Api.Services;

/// <summary>
/// Extracts plain text from DOCX files using Open XML SDK.
/// </summary>
public class DocxTextExtractor : ICvTextExtractor
{
    private readonly ILogger<DocxTextExtractor> _logger;

    public string SupportedContentType => "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    public DocxTextExtractor(ILogger<DocxTextExtractor> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExtractTextAsync(Stream fileStream, string contentType)
    {
        if (contentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        {
            throw new NotSupportedException($"Content type '{contentType}' is not supported by DocxTextExtractor.");
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var document = WordprocessingDocument.Open(memoryStream, false);
            
            if (document.MainDocumentPart is null)
            {
                throw new InvalidOperationException("DOCX file does not contain a main document part");
            }

            var textBuilder = new System.Text.StringBuilder();

            var body = document.MainDocumentPart.Document.Body;
            if (body is not null)
            {
                foreach (var paragraph in body.Descendants<Paragraph>())
                {
                    var paragraphText = paragraph.InnerText;
                    if (!string.IsNullOrWhiteSpace(paragraphText))
                    {
                        textBuilder.AppendLine(paragraphText);
                    }
                }
            }

            foreach (var table in body?.Descendants<Table>() ?? Enumerable.Empty<Table>())
            {
                foreach (var cell in table.Descendants<TableCell>())
                {
                    var cellText = cell.InnerText;
                    if (!string.IsNullOrWhiteSpace(cellText))
                    {
                        textBuilder.Append(cellText).Append(" ");
                    }
                }
                textBuilder.AppendLine();
            }

            var extractedText = textBuilder.ToString().Trim();

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                _logger.LogWarning("DOCX text extraction resulted in empty text");
            }
            else
            {
                _logger.LogInformation("Successfully extracted {Length} characters from DOCX", extractedText.Length);
            }

            return extractedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract text from DOCX");
            throw new InvalidOperationException("Failed to extract text from DOCX file", ex);
        }
    }
}
