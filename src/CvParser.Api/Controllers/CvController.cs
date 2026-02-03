using CvParser.Api.DTOs;
using CvParser.Core.Interfaces;
using CvParser.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CvParser.Api.Controllers;

/// <summary>
/// API controller for CV document operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CvController : ControllerBase
{
    private readonly ICvParserService _parserService;
    private readonly IDocumentRepository _repository;
    private readonly ILogger<CvController> _logger;

    public CvController(
        ICvParserService parserService,
        IDocumentRepository repository,
        ILogger<CvController> logger)
    {
        _parserService = parserService;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Upload and parse a CV document
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(CvUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CvUploadResponse>> UploadCv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var document = await _parserService.ParseCvAsync(stream, file.FileName);
            
            await _repository.SaveAsync(document);

            var response = new CvUploadResponse
            {
                DocumentId = document.Id,
                FileName = document.FileName,
                UploadedAt = document.UploadedAt,
                ExtractedSkills = document.Skills.Select(s => new SkillDto
                {
                    Name = s.Name,
                    Category = s.Category,
                    YearsOfExperience = s.YearsOfExperience,
                    Level = s.Level.ToString()
                }).ToList()
            };

            _logger.LogInformation("Successfully processed CV: {FileName} with {SkillCount} skills", 
                file.FileName, document.Skills.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing CV: {FileName}", file.FileName);
            return StatusCode(500, "Error processing CV");
        }
    }

    /// <summary>
    /// Get all uploaded CV documents
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CvDocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CvDocumentDto>>> GetAllDocuments()
    {
        var documents = await _repository.GetAllAsync();
        var dtos = documents.Select(MapToDto).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Get a specific CV document by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CvDocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CvDocumentDto>> GetDocument(Guid id)
    {
        var document = await _repository.GetByIdAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(document));
    }

    /// <summary>
    /// Delete a CV document
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static CvDocumentDto MapToDto(CvDocument document)
    {
        return new CvDocumentDto
        {
            Id = document.Id,
            FileName = document.FileName,
            FullName = document.FullName,
            Email = document.Email,
            PhoneNumber = document.PhoneNumber,
            UploadedAt = document.UploadedAt,
            ParsedAt = document.ParsedAt,
            Skills = document.Skills.Select(s => new SkillDto
            {
                Name = s.Name,
                Category = s.Category,
                YearsOfExperience = s.YearsOfExperience,
                Level = s.Level.ToString()
            }).ToList()
        };
    }
}
