using PersonalDigitalVault.Api.Entities;


namespace PersonalDigitalVault.Api.Interfaces.Repositories;

public interface IDocumentRepository
{
    // Logged-in user documents
    Task<List<Document>> GetByUserAsync(int userId);

    // Specific document owner check
    Task<Document?> GetOwnedAsync(
        int id,
        int userId);

    // Document add
    Task AddAsync(Document document);

    // Document update
    Task UpdateAsync(Document document);

    // Document delete
    Task DeleteAsync(Document document);

    // Admin dashboard total documents count
    Task<int> CountAsync();

    // Admin upload activity-ku metadata edukkanum
    // User navigation-um include pannuvom
    Task<List<Document>> GetAllForAdminAsync();
}