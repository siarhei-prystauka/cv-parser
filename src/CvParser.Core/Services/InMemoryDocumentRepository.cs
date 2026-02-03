using CvParser.Core.Interfaces;
using CvParser.Core.Models;

namespace CvParser.Core.Services;

/// <summary>
/// In-memory implementation of document repository
/// </summary>
public class InMemoryDocumentRepository : IDocumentRepository
{
    private readonly Dictionary<Guid, CvDocument> _documents = new();
    
    public Task<CvDocument> SaveAsync(CvDocument document)
    {
        _documents[document.Id] = document;
        return Task.FromResult(document);
    }
    
    public Task<CvDocument?> GetByIdAsync(Guid id)
    {
        _documents.TryGetValue(id, out var document);
        return Task.FromResult(document);
    }
    
    public Task<List<CvDocument>> GetAllAsync()
    {
        return Task.FromResult(_documents.Values.ToList());
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_documents.Remove(id));
    }
}
