using CvParser.Api.Converters;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "CV Parser API", Version = "v1" });
    
    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Register text extraction services
builder.Services.AddScoped<PdfTextExtractor>();
// TODO: Add DocxTextExtractor when DOCX support is implemented (see GitHub issue)
builder.Services.AddSingleton<CvTextExtractorFactory>();

// Register Groq HTTP client with retry policy
builder.Services.AddHttpClient<ILlmSkillExtractor, GroqSkillExtractor>(client =>
{
    var baseUrl = builder.Configuration["Groq:BaseUrl"] ?? "https://api.groq.com/openai/v1";
    client.BaseAddress = new Uri(baseUrl);
    
    var timeoutSeconds = builder.Configuration.GetValue<int>("Groq:TimeoutSeconds", 30);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
})
.AddPolicyHandler(GetRetryPolicy());

// Register application services
builder.Services.AddSingleton<IProfileRepository, InMemoryProfileRepository>();
builder.Services.AddScoped<ICvSkillExtractor, HybridCvSkillExtractor>();
builder.Services.AddScoped<IProfileConverter, ProfileConverter>();

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CV Parser API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Gets a retry policy for resilient HTTP calls to Groq API.
/// </summary>
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5)
        });
}
