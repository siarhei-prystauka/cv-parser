namespace CvParser.Api.Services;

/// <summary>
/// Contract for extracting skills using an LLM.
/// </summary>
public interface ILlmSkillExtractor
{
    /// <summary>
    /// Extracts skills from CV text using an LLM.
    /// </summary>
    /// <param name="cvText">The CV text to analyze.</param>
    /// <returns>A list of extracted skill names.</returns>
    Task<IEnumerable<string>> ExtractSkillsAsync(string cvText);
}
