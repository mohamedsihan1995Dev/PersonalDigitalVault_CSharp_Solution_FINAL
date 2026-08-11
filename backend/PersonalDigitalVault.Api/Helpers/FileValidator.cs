namespace PersonalDigitalVault.Api.Helpers;
public static class FileValidator
{
    private static readonly string[] Allowed = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" }; private const long MaxBytes = 10 * 1024 * 1024;
    public static void Validate(IFormFile file) { if (file is null || file.Length == 0) throw new ArgumentException("Please select a file."); if (file.Length > MaxBytes) throw new ArgumentException("File is larger than 10 MB."); var ext = Path.GetExtension(file.FileName).ToLowerInvariant(); if (!Allowed.Contains(ext)) throw new ArgumentException("Unsupported file type."); }
}
