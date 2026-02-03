using CvParser.Core.Models;
using CvParser.Core.Services;

namespace CvParser.Tests;

public class InMemoryDocumentRepositoryTests
{
    [Fact]
    public async Task SaveAsync_WithValidDocument_SavesDocument()
    {
        // Arrange
        var repository = new InMemoryDocumentRepository();
        var document = new CvDocument
        {
            FileName = "test.txt",
            FullName = "John Doe"
        };

        // Act
        var result = await repository.SaveAsync(document);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(document.Id, result.Id);
        Assert.Equal(document.FileName, result.FileName);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsDocument()
    {
        // Arrange
        var repository = new InMemoryDocumentRepository();
        var document = new CvDocument { FileName = "test.txt" };
        await repository.SaveAsync(document);

        // Act
        var result = await repository.GetByIdAsync(document.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(document.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var repository = new InMemoryDocumentRepository();

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleDocuments_ReturnsAllDocuments()
    {
        // Arrange
        var repository = new InMemoryDocumentRepository();
        await repository.SaveAsync(new CvDocument { FileName = "doc1.txt" });
        await repository.SaveAsync(new CvDocument { FileName = "doc2.txt" });

        // Act
        var results = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var repository = new InMemoryDocumentRepository();
        var document = new CvDocument { FileName = "test.txt" };
        await repository.SaveAsync(document);

        // Act
        var result = await repository.DeleteAsync(document.Id);

        // Assert
        Assert.True(result);
        Assert.Null(await repository.GetByIdAsync(document.Id));
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        var repository = new InMemoryDocumentRepository();

        // Act
        var result = await repository.DeleteAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}
