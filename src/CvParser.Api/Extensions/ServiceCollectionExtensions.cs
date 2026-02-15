using CvParser.Api.Converters;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Polly;
using Polly.Extensions.Http;

namespace CvParser.Api.Extensions;

/// <summary>
/// Registers application services in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICvTextExtractor, PdfTextExtractor>();
        // TODO: Register DocxTextExtractor when DOCX support is implemented (see GitHub issue #5)
        // services.AddScoped<ICvTextExtractor, DocxTextExtractor>();

        services.AddScoped<ICvTextExtractorFactory, CvTextExtractorFactory>();

        services.AddHttpClient<ILlmSkillExtractor, GroqSkillExtractor>(client =>
        {
            var baseUrl = configuration["Groq:BaseUrl"] ?? "https://api.groq.com/openai/v1/";
            // Ensure trailing slash for proper URI combining
            if (!baseUrl.EndsWith('/'))
            {
                baseUrl += "/";
            }
            client.BaseAddress = new Uri(baseUrl);

            var timeoutSeconds = configuration.GetValue<int>("Groq:TimeoutSeconds", 30);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
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
