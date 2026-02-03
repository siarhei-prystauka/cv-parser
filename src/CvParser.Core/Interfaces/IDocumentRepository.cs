using CvParser.Core.Models;

namespace CvParser.Core.Interfaces;

/// <summary>
/// Repository for storing and retrieving CV documents
/// </summary>
public interface IDocumentRepository
{
    Task<CvDocument> SaveAsync(CvDocument document);
    Task<CvDocument?> GetByIdAsync(Guid id);
    Task<List<CvDocument>> GetAllAsync();
    Task<bool> DeleteAsync(Guid id);
}
