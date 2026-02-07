using CvParser.Api.Endpoints;
using CvParser.Api.Repositories;
using CvParser.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSingleton<IProfileRepository, InMemoryProfileRepository>();
builder.Services.AddSingleton<ICvSkillExtractor, MockCvSkillExtractor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("dev");

app.MapProfileEndpoints();

app.Run();
