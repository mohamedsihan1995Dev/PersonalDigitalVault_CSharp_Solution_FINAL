namespace PersonalDigitalVault.Api.Entities;

public class Document
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileHash { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }
    public int? FolderId { get; set; }
    public Folder? Folder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
