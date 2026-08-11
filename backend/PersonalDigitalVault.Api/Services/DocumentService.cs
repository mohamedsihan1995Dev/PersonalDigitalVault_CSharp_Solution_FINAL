using PersonalDigitalVault.Api.DTOs.Document;
using PersonalDigitalVault.Api.Entities;
using PersonalDigitalVault.Api.Helpers;
using PersonalDigitalVault.Api.Interfaces.Repositories;
using PersonalDigitalVault.Api.Interfaces.Services;
using PersonalDigitalVault.Api.Security;
using PersonalDigitalVault.Api.Storage;
namespace PersonalDigitalVault.Api.Services;

public class DocumentService(IDocumentRepository repo, IFolderRepository folders, CurrentUserService current, AesEncryption aes, FileHashService hash, FileStorageService storage) : IDocumentService
{
    private static DocumentDto Map(Document x) => new() { Id = x.Id, FileName = x.OriginalFileName, ContentType = x.ContentType, FileSize = x.FileSize, FileHash = x.FileHash, FolderId = x.FolderId, CreatedAt = x.CreatedAt };
    public async Task<DocumentDto> UploadAsync(UploadDocumentDto dto) { FileValidator.Validate(dto.File); if (dto.FolderId.HasValue && await folders.GetOwnedAsync(dto.FolderId.Value, current.UserId) is null) throw new ArgumentException("Selected folder does not belong to the current user."); using var ms = new MemoryStream(); await dto.File.CopyToAsync(ms); var plain = ms.ToArray(); var stored = $"{Guid.NewGuid():N}.vault"; var encrypted = aes.EncryptBytes(plain); var path = await storage.SaveAsync(stored, encrypted); var x = new Document { OriginalFileName = Path.GetFileName(dto.File.FileName), StoredFileName = stored, ContentType = dto.File.ContentType, FileSize = dto.File.Length, FileHash = hash.Sha256(plain), StoragePath = path, FolderId = dto.FolderId, UserId = current.UserId }; await repo.AddAsync(x); return Map(x); }
    public async Task<List<DocumentDto>> GetAllAsync() => (await repo.GetByUserAsync(current.UserId)).Select(Map).ToList(); public async Task<DocumentDto> GetAsync(int id) => Map(await repo.GetOwnedAsync(id, current.UserId) ?? throw new KeyNotFoundException("Document not found."));
    public async Task<(byte[] Data, string ContentType, string FileName)> DownloadAsync(int id) { var x = await repo.GetOwnedAsync(id, current.UserId) ?? throw new KeyNotFoundException("Document not found."); var encrypted = await storage.ReadAsync(x.StoragePath); var plain = aes.DecryptBytes(encrypted); if (!string.Equals(hash.Sha256(plain), x.FileHash, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("File integrity check failed."); return (plain, x.ContentType, x.OriginalFileName); }
    public async Task<DocumentDto> UpdateAsync(int id, UpdateDocumentDto dto) { var x = await repo.GetOwnedAsync(id, current.UserId) ?? throw new KeyNotFoundException("Document not found."); x.OriginalFileName = Path.GetFileName(dto.FileName); x.UpdatedAt = DateTime.UtcNow; await repo.UpdateAsync(x); return Map(x); }
    public async Task DeleteAsync(int id) { var x = await repo.GetOwnedAsync(id, current.UserId) ?? throw new KeyNotFoundException("Document not found."); storage.Delete(x.StoragePath); await repo.DeleteAsync(x); }
}
