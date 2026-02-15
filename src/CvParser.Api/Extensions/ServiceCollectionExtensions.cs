using CvParser.Api.Converters;
using CvParser.Api.Models.Options;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace CvParser.Api.Extensions;

/// <summary>
/// Registers application services in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddOptions<GroqOptions>()
            .BindConfiguration(GroqOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<FileValidationOptions>()
            .BindConfiguration(FileValidationOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SkillExtractionOptions>()
            .BindConfiguration(SkillExtractionOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ISettingsRepository>(serviceProvider =>
        {
            var groqOptions = serviceProvider.GetRequiredService<IOptions<GroqOptions>>().Value;
            var skillExtractionOptions = serviceProvider.GetRequiredService<IOptions<SkillExtractionOptions>>().Value;
            return new InMemorySettingsRepository(skillExtractionOptions, groqOptions);
        });

        services.AddScoped<ICvTextExtractor, PdfTextExtractor>();
        // TODO: Register DocxTextExtractor when DOCX support is implemented (see GitHub issue #5)
        // services.AddScoped<ICvTextExtractor, DocxTextExtractor>();

        services.AddScoped<ICvTextExtractorFactory, CvTextExtractorFactory>();

        services.AddHttpClient<ILlmSkillExtractor, GroqSkillExtractor>((serviceProvider, client) =>
        {
            var settingsRepository = serviceProvider.GetRequiredService<ISettingsRepository>();
            var groqOptions = settingsRepository.GetGroqOptionsAsync().GetAwaiter().GetResult();
            
            var baseUrl = groqOptions.BaseUrl;
            // Ensure trailing slash for proper URI combining
            if (!baseUrl.EndsWith('/'))
            {
                baseUrl += "/";
            }
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(groqOptions.TimeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy());

        services.AddSingleton<IProfileRepository, InMemoryProfileRepository>();
        services.AddScoped<ICvSkillExtractor, HybridCvSkillExtractor>();
        services.AddScoped<IProfileConverter, ProfileConverter>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5)
            });
    }
}
