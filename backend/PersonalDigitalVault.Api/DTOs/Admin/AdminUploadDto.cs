namespace PersonalDigitalVault.Api.DTOs.Admin;

public class AdminUploadDto
{
  
    public string UploadedBy { get; set; } = string.Empty;

   
    public string FileName { get; set; } = string.Empty;

   
    public long FileSize { get; set; }

    // File upload date/time
    public DateTime UploadedAt { get; set; }
}