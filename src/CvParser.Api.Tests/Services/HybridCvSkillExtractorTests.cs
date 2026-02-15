using CvParser.Api.Models.Options;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CvParser.Api.Tests.Services;

/// <summary>
/// Tests for <see cref="HybridCvSkillExtractor"/>.
/// </summary>
public sealed class HybridCvSkillExtractorTests
{
    private ICvTextExtractorFactory _textExtractorFactory = null!;
    private ILlmSkillExtractor _llmExtractor = null!;
    private IWebHostEnvironment _environment = null!;
    private ISettingsRepository _settingsRepository = null!;
    private ILogger<HybridCvSkillExtractor> _logger = null!;
    private ICvTextExtractor _textExtractor = null!;

    [SetUp]
    public void SetUp()
    {
        _textExtractorFactory = Substitute.For<ICvTextExtractorFactory>();
        _llmExtractor = Substitute.For<ILlmSkillExtractor>();
        _environment = Substitute.For<IWebHostEnvironment>();
        _settingsRepository = Substitute.For<ISettingsRepository>();
        _logger = Substitute.For<ILogger<HybridCvSkillExtractor>>();
        _textExtractor = Substitute.For<ICvTextExtractor>();

        var testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData");
        _environment.ContentRootPath.Returns(testDataPath);

        _textExtractorFactory.GetExtractor(Arg.Any<string>()).Returns(_textExtractor);
    }

    [Test]
    public async Task ExtractSkillsAsync_WhenLlmFallbackOnlyIsFalse_CallsLlmExtractor()
    {
        var options = new SkillExtractionOptions { LlmFallbackOnly = false };
        _settingsRepository.GetSkillExtractionOptionsAsync().Returns(Task.FromResult(options));

        var cvText = "I have experience with C# and React";
        _textExtractor.ExtractTextAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(Task.FromResult(cvText));

        var llmSkills = new List<string> { "C#", "React", "Entity Framework" };
        _llmExtractor.ExtractSkillsAsync(cvText, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<string>>(llmSkills));

        var extractor = new HybridCvSkillExtractor(
            _textExtractorFactory,
            _llmExtractor,
            _environment,
            _settingsRepository,
            _logger);

        using var stream = new MemoryStream();
        var result = await extractor.ExtractSkillsAsync(stream, "test.pdf", "application/pdf");

        await _llmExtractor.Received(1).ExtractSkillsAsync(cvText, Arg.Any<CancellationToken>());
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain("C#"));
        Assert.That(result, Does.Contain("React"));
    }

    [Test]
    public async Task ExtractSkillsAsync_WhenLlmFallbackOnlyIsTrueAndTaxonomyFindsSkills_DoesNotCallLlmExtractor()
    {
        var options = new SkillExtractionOptions { LlmFallbackOnly = true };
        _settingsRepository.GetSkillExtractionOptionsAsync().Returns(Task.FromResult(options));

        var cvText = "I have experience with C# and React frameworks";
        _textExtractor.ExtractTextAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(Task.FromResult(cvText));

        var llmSkills = new List<string> { "Should not be called" };
        _llmExtractor.ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<string>>(llmSkills));

        var extractor = new HybridCvSkillExtractor(
            _textExtractorFactory,
            _llmExtractor,
            _environment,
            _settingsRepository,
            _logger);

        using var stream = new MemoryStream();
        var result = await extractor.ExtractSkillsAsync(stream, "test.pdf", "application/pdf");

        await _llmExtractor.DidNotReceive().ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain("C#"));
        Assert.That(result, Does.Contain("React"));
        Assert.That(result, Does.Not.Contain("Should not be called"));
    }

    [Test]
    public async Task ExtractSkillsAsync_WhenLlmFallbackOnlyIsTrueAndTaxonomyFindsNoSkills_CallsLlmExtractor()
    {
        var options = new SkillExtractionOptions { LlmFallbackOnly = true };
        _settingsRepository.GetSkillExtractionOptionsAsync().Returns(Task.FromResult(options));

        var cvText = "I have experience with Rust and Go programming languages";
        _textExtractor.ExtractTextAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(Task.FromResult(cvText));

        var llmSkills = new List<string> { "Rust", "Go" };
        _llmExtractor.ExtractSkillsAsync(cvText, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<string>>(llmSkills));

        var extractor = new HybridCvSkillExtractor(
            _textExtractorFactory,
            _llmExtractor,
            _environment,
            _settingsRepository,
            _logger);

        using var stream = new MemoryStream();
        var result = await extractor.ExtractSkillsAsync(stream, "test.pdf", "application/pdf");

        await _llmExtractor.Received(1).ExtractSkillsAsync(cvText, Arg.Any<CancellationToken>());
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Does.Contain("Rust"));
        Assert.That(result, Does.Contain("Go"));
    }

    [Test]
    public async Task ExtractSkillsAsync_WhenTaxonomyMatchesAlias_ReturnsCanonicalnName()
    {
        var options = new SkillExtractionOptions { LlmFallbackOnly = true };
        _settingsRepository.GetSkillExtractionOptionsAsync().Returns(Task.FromResult(options));

        var cvText = "I know csharp and reactjs";
        _textExtractor.ExtractTextAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(Task.FromResult(cvText));

        var extractor = new HybridCvSkillExtractor(
            _textExtractorFactory,
            _llmExtractor,
            _environment,
            _settingsRepository,
            _logger);

        using var stream = new MemoryStream();
        var result = await extractor.ExtractSkillsAsync(stream, "test.pdf", "application/pdf");

        await _llmExtractor.DidNotReceive().ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        Assert.That(result, Does.Contain("C#"));
        Assert.That(result, Does.Contain("React"));
        Assert.That(result, Does.Not.Contain("csharp"));
        Assert.That(result, Does.Not.Contain("reactjs"));
    }

    [Test]
    public async Task ExtractSkillsAsync_WithDuplicateSkills_ReturnsUniqueSkills()
    {
        var options = new SkillExtractionOptions { LlmFallbackOnly = false };
        _settingsRepository.GetSkillExtractionOptionsAsync().Returns(Task.FromResult(options));

        var cvText = "I use C# and csharp programming language";
        _textExtractor.ExtractTextAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(Task.FromResult(cvText));

        var llmSkills = new List<string> { "C#", "c#" };
        _llmExtractor.ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<string>>(llmSkills));

        var extractor = new HybridCvSkillExtractor(
            _textExtractorFactory,
            _llmExtractor,
            _environment,
            _settingsRepository,
            _logger);

        using var stream = new MemoryStream();
        var result = await extractor.ExtractSkillsAsync(stream, "test.pdf", "application/pdf");

        Assert.That(result.Count(s => s.Equals("C#", StringComparison.OrdinalIgnoreCase)), Is.EqualTo(1));
    }

    [Test]
    public async Task ExtractSkillsAsync_WithEmptyText_ReturnsEmptyList()
    {
        var options = new SkillExtractionOptions { LlmFallbackOnly = false };
        _settingsRepository.GetSkillExtractionOptionsAsync().Returns(Task.FromResult(options));

        _textExtractor.ExtractTextAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(Task.FromResult(string.Empty));

        var extractor = new HybridCvSkillExtractor(
            _textExtractorFactory,
            _llmExtractor,
            _environment,
            _settingsRepository,
            _logger);

        using var stream = new MemoryStream();
        var result = await extractor.ExtractSkillsAsync(stream, "test.pdf", "application/pdf");

        await _llmExtractor.DidNotReceive().ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        Assert.That(result, Is.Empty);
    }
}
