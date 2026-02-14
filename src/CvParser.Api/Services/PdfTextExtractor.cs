using UglyToad.PdfPig;

namespace CvParser.Api.Services;

/// <summary>
/// Extracts plain text from PDF files using PdfPig.
/// </summary>
public class PdfTextExtractor : ICvTextExtractor
{
    private readonly ILogger<PdfTextExtractor> _logger;

    public PdfTextExtractor(ILogger<PdfTextExtractor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extracts text from a PDF file stream.
    /// </summary>
    public async Task<string> ExtractTextAsync(Stream fileStream, string contentType)
    {
        if (contentType != "application/pdf")
        {
            throw new NotSupportedException($"Content type '{contentType}' is not supported by PdfTextExtractor.");
        }

        try
        {
            // PdfPig requires a seekable stream
            var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var document = PdfDocument.Open(memoryStream);
            var textBuilder = new System.Text.StringBuilder();

            foreach (var page in document.GetPages())
            {
                // Use page.Text for basic text extraction
                var text = page.Text;
                textBuilder.AppendLine(text);
            }

            var extractedText = textBuilder.ToString().Trim();

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                _logger.LogWarning("PDF text extraction resulted in empty text");
            }
            else
            {
                _logger.LogInformation("Successfully extracted {Length} characters from PDF", extractedText.Length);
            }

            return extractedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract text from PDF");
            throw new InvalidOperationException("Failed to extract text from PDF file", ex);
        }
    }
}
