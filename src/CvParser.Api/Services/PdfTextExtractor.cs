using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace CvParser.Api.Services;

/// <summary>
/// Extracts plain text from PDF files using iText7.
/// </summary>
public class PdfTextExtractor : ICvTextExtractor
{
    private readonly ILogger<PdfTextExtractor> _logger;

    public string SupportedContentType => "application/pdf";

    public PdfTextExtractor(ILogger<PdfTextExtractor> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExtractTextAsync(Stream fileStream, string contentType)
    {
        if (contentType != "application/pdf")
        {
            throw new NotSupportedException($"Content type '{contentType}' is not supported by PdfTextExtractor.");
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var pdfReader = new PdfReader(memoryStream);
            using var pdfDocument = new PdfDocument(pdfReader);
            
            var textBuilder = new System.Text.StringBuilder();
            
            for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                textBuilder.AppendLine(pageText);
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
