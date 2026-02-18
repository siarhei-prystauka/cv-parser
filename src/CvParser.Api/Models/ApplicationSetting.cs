namespace CvParser.Api.Models;

public sealed class ApplicationSetting
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTime UpdatedAt { get; set; }
}
