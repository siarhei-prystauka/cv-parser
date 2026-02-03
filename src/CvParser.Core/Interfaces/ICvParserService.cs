using CvParser.Core.Models;

namespace CvParser.Core.Interfaces;

/// <summary>
/// Service for parsing CV documents and extracting structured information
/// </summary>
public interface ICvParserService
{
    /// <summary>
    /// Parse a CV document from a stream
    /// </summary>
    Task<CvDocument> ParseCvAsync(Stream documentStream, string fileName);
    
    /// <summary>
    /// Extract skills from parsed CV text
    /// </summary>
    Task<List<Skill>> ExtractSkillsAsync(string cvText);
}
