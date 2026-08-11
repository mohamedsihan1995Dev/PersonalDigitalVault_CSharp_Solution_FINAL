using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Api.Data;
using PersonalDigitalVault.Api.Entities;
using PersonalDigitalVault.Api.Interfaces.Repositories;
using System.Reflection.Metadata;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace PersonalDigitalVault.Api.Repositories;

public class DocumentRepository(AppDbContext db)
    : IDocumentRepository
{
    // =====================================================
    // USER DOCUMENT LIST
    // =====================================================
    // INPUT:
    // userId -> logged-in user id
    //
    // OUTPUT:
    // அந்த user-ku belong aagura documents mattum return pannum
    public Task<List<Document>> GetByUserAsync(int userId)
    {
        return db.Documents
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }


    // =====================================================
    // OWNED DOCUMENT
    // =====================================================
    // INPUT:
    // id     -> document id
    // userId -> logged-in user id
    //
    // REASON:
    // மற்ற user document access panna முடியாத மாதிரி ownership check
    //
    // OUTPUT:
    // match aana document / illaina null
    public Task<Document?> GetOwnedAsync(
        int id,
        int userId)
    {
        return db.Documents
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.UserId == userId);
    }


    // =====================================================
    // ADD DOCUMENT
    // =====================================================
    public async Task AddAsync(Document document)
    {
        db.Documents.Add(document);

        await db.SaveChangesAsync();
    }


    // =====================================================
    // UPDATE DOCUMENT
    // =====================================================
    public async Task UpdateAsync(Document document)
    {
        db.Documents.Update(document);

        await db.SaveChangesAsync();
    }


    // =====================================================
    // DELETE DOCUMENT
    // =====================================================
    public async Task DeleteAsync(Document document)
    {
        db.Documents.Remove(document);

        await db.SaveChangesAsync();
    }


    // =====================================================
    // TOTAL DOCUMENT COUNT
    // =====================================================
    // Admin dashboard-ku total stored document count
    public Task<int> CountAsync()
    {
        return db.Documents.CountAsync();
    }


    // =====================================================
    // ADMIN UPLOAD ACTIVITY
    // =====================================================
    // Admin-ku:
    // Uploaded user name
    // File name
    // File size
    // Uploaded date
 
    //
    // File open/download/decrypt details return panna maatom.
    public Task<List<Document>> GetAllForAdminAsync()
    {
        return db.Documents
            .AsNoTracking()

            // Document.User relationship load pannum
            .Include(x => x.User)

            // Latest uploads first
            .OrderByDescending(x => x.CreatedAt)

            .ToListAsync();
    }
}