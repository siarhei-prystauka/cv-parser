using System;
using CvParser.Api.Converters;
using CvParser.Api.Data;
using CvParser.Api.Models.Options;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace CvParser.Api.Extensions;

/// <summary>
/// Registers application services in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        ValidateOrigins(allowedOrigins);

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

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (connectionString?.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase) == true)
                options.UseSqlite(connectionString);
            else
                options.UseSqlServer(connectionString);
        });

        services.AddScoped<ISettingsRepository, SqlSettingsRepository>();

        services.AddScoped<ICvTextExtractor, PdfTextExtractor>();
        // TODO: Register DocxTextExtractor when DOCX support is implemented (see GitHub issue #5)
        // services.AddScoped<ICvTextExtractor, DocxTextExtractor>();

        services.AddScoped<ICvTextExtractorFactory, CvTextExtractorFactory>();

        services.AddHttpClient<ILlmSkillExtractor, GroqSkillExtractor>((serviceProvider, client) =>
        {
            var groqOptions = serviceProvider.GetRequiredService<IOptions<GroqOptions>>().Value;
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

        services.AddScoped<IProfileRepository, SqlProfileRepository>();
        services.AddScoped<ICvSkillExtractor, HybridCvSkillExtractor>();
        services.AddScoped<IProfileConverter, ProfileConverter>();
        
        services.AddSingleton<ITaxonomyService, TaxonomyService>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    policy.SetIsOriginAllowed(_ => false);
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Validates that all configured origins are valid URIs starting with http:// or https://.
    /// </summary>
    /// <param name="origins">Array of origin URIs to validate.</param>
    /// <exception cref="ArgumentException">Thrown if any origin is invalid.</exception>
    private static void ValidateOrigins(string[] origins)
    {
        if (origins.Length == 0)
        {
            return;
        }

        foreach (var origin in origins)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                throw new ArgumentException("AllowedOrigins contains an empty or whitespace-only value.", nameof(origins));
            }

            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException($"AllowedOrigins contains an invalid URI: '{origin}'.", nameof(origins));
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException($"AllowedOrigins contains a URI with invalid scheme '{uri.Scheme}'. Only 'http' and 'https' are allowed: '{origin}'.", nameof(origins));
            }

            if (origin.Contains('*'))
            {
                throw new ArgumentException($"AllowedOrigins contains wildcards, which are not allowed for security reasons: '{origin}'.", nameof(origins));
            }
        }
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
